using System;
using UnityEngine;

/// <summary>
///     ピースの抽選と生成を管理するクラス
/// </summary>
public class PieceSpawner : MonoBehaviour
{
    /// <summary> 次に生成されるピース定義 </summary>
    public PieceDefinition NextPieceDefinition => _nextPieceDefinition;

    /// <summary> 次に生成されるピース定義が変更されたときに発火するイベント </summary>
    public event Action<PieceDefinition> OnNextPieceDefinitionChanged;

    /// <summary>
    ///     ピースの生成処理に必要な設定を初期化する
    /// </summary>
    /// <param name="stageSettings">ステージ設定</param>
    /// <param name="characterDefinition">キャラクター定義</param>
    /// <returns>初期化に成功したかどうか</returns>
    public bool Initialize(
        StageSettings stageSettings,
        CharacterDefinition characterDefinition)
    {
        _stageSettings = stageSettings;
        _characterDefinition = characterDefinition;
        _nextPieceDefinition = null;

        if (!ValidateSettings())
        {
            return false;
        }

        // スポーンポイントの初期位置を保存する
        _initialSpawnPointY = _spawnPoint != null
            ? _spawnPoint.position.y
            : 0f;

        _highestReachedStep = 0;

        return true;
    }

    /// <summary>
    ///     現在の高さと抽選補正に応じたピースを生成する
    /// </summary>
    /// <param name="currentHeight">現在の高さ</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>生成されたピース</returns>
    public FallingPiece SpawnPiece(
        float currentHeight,
        PieceSpawnModifiers spawnModifiers)
    {
        if (_stageSettings == null)
        {
            Debug.LogError("PieceSpawnerが初期化されていません。", this);
            return null;
        }

        // 現在の高さに応じたSpawnTableを取得する
        HeightSpawnTable spawnTable = GetSpawnTable(currentHeight);

        if (spawnTable == null)
        {
            return null;
        }

        // 次に生成するピース定義が未設定の場合は、抽選して設定する
        if (_nextPieceDefinition == null)
        {
            _nextPieceDefinition = SelectPieceDefinition(
                spawnTable,
                spawnModifiers);
        }

        PieceDefinition currentDefinition = _nextPieceDefinition;

        if (currentDefinition == null)
        {
            Debug.LogError($"生成するピース定義を取得できませんでした。", this);
            return null;
        }

        // 次に生成するピース定義を抽選して設定する
        _nextPieceDefinition = SelectPieceDefinition(
            spawnTable,
            spawnModifiers);

        // 次に生成するピース定義が変更されたことを通知する
        OnNextPieceDefinitionChanged?.Invoke(_nextPieceDefinition);
        UpdateSpawnPointPosition(currentHeight);

        // ピースを生成する
        FallingPiece prefab = currentDefinition.PrefabOverride != null
            ? currentDefinition.PrefabOverride
            : _defaultPiecePrefab;

        FallingPiece piece = Instantiate(
            prefab,
            _spawnPoint.position,
            Quaternion.identity);

        piece.Initialize(
            currentDefinition,
            GetGlobalPiecePhysicsSettings(),
            _stageSettings.DeletePositionY);

        return piece;
    }

    /// <summary>
    ///     指定されたピース定義、位置、回転に基づいてピースを生成する
    /// </summary>
    /// <param name="pieceDefinition">ピース定義</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成回転</param>
    /// <returns>生成されたピース</returns>
    public FallingPiece CreatePiece(
        PieceDefinition pieceDefinition,
        Vector3 position,
        Quaternion rotation)
    {
        if (pieceDefinition == null)
        {
            Debug.LogError("生成するピース定義が設定されていません。", this);
            return null;
        }

        // ピースを生成する
        FallingPiece prefab = pieceDefinition.PrefabOverride != null
            ? pieceDefinition.PrefabOverride
            : _defaultPiecePrefab;

        FallingPiece piece = Instantiate(
            prefab,
            position,
            rotation);
        piece.Initialize(
            pieceDefinition,
            GetGlobalPiecePhysicsSettings(),
            _stageSettings.DeletePositionY);
        return piece;
    }

    /// <summary>
    ///     現在の高さに応じたSpawnTableとPieceTypeに基づいて、ランダムにピース定義を取得する
    /// </summary>
    /// <param name="height">現在の高さ</param>
    /// <param name="pieceType">ピースの種類</param>
    /// <returns>ランダムに選択されたピース定義</returns>
    public PieceDefinition GetRandomPieceDefinition(float height, PieceType pieceType)
    {
        HeightSpawnTable spawnTable = GetSpawnTable(height);

        if (spawnTable == null)
        {
            return null;
        }

        return SelectPieceByWeight(
            spawnTable.CastlePartType,
            pieceType);
    }

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private FallingPiece _defaultPiecePrefab;
    [SerializeField, Min(0.1f)] private float _spawnPointHeightStep = 5f;

    private StageSettings _stageSettings;
    private CharacterDefinition _characterDefinition;
    private PieceDefinition _nextPieceDefinition;
    private float _initialSpawnPointY;
    private int _highestReachedStep;

    /// <summary>
    ///     SpawnTableと抽選補正に応じたピース定義を抽選して取得する
    /// </summary>
    /// <param name="spawnTable">スポーンテーブル</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>抽選されたピース定義</returns>
    private PieceDefinition SelectPieceDefinition(
        HeightSpawnTable spawnTable,
        PieceSpawnModifiers spawnModifiers)
    {
        // SpawnTableと抽選補正に応じたPieceTypeを抽選する
        PieceType? pieceType = SelectPieceTypeByWeight(
            spawnTable,
            spawnModifiers);

        if (!pieceType.HasValue)
        {
            return null;
        }

        return SelectPieceByWeight(
            spawnTable.CastlePartType,
            pieceType.Value);
    }

    /// <summary>
    ///     スポーンポイントの位置を現在の高さに応じて更新する
    /// </summary>
    /// <param name="currentHeight">現在の高さ</param>
    private void UpdateSpawnPointPosition(float currentHeight)
    {
        if (_spawnPoint == null)
        {
            return;
        }

        int currentStep = Mathf.Max(
            0,
            Mathf.FloorToInt((currentHeight) / _spawnPointHeightStep));

        if (currentStep == _highestReachedStep)
        {
            return;
        }

        _highestReachedStep = currentStep;

        Vector3 position = _spawnPoint.position;
        position.y = _initialSpawnPointY + _highestReachedStep * _spawnPointHeightStep;
        _spawnPoint.position = position;
    }

    /// <summary>
    ///     現在の高さに応じたSpawnTableを取得する
    /// </summary>
    /// <param name="height">現在の高さ</param>
    /// <returns>選択されたHeightSpawnTable</returns>
    private HeightSpawnTable GetSpawnTable(float height)
    {
        HeightSpawnTable spawnTable = null;

        // 高さに応じたSpawnTableを取得する
        foreach (var table in _stageSettings.HeightSpawnTables)
        {
            // 高さが閾値を超えていない場合はスキップする
            if (table == null ||
                height < table.HeightThreshold)
            {
                continue;
            }

            // 高さが閾値を超えている場合は、最も高い閾値のSpawnTableを選択する
            if (spawnTable == null ||
                table.HeightThreshold > spawnTable.HeightThreshold)
            {
                spawnTable = table;
            }
        }

        // SpawnTableが見つかった場合は返す
        if (spawnTable != null)
        {
            return spawnTable;
        }

        Debug.LogError($"HeightSpawnTableが見つかりませんでした。現在の高さ: {height}", this);

        return null;
    }

    /// <summary>
    ///     SpawnTableと抽選補正に応じたPieceTypeを抽選して取得する
    /// </summary>
    /// <param name="spawnTable">スポーンテーブル</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>抽選されたPieceType</returns>
    private PieceType? SelectPieceTypeByWeight(
        HeightSpawnTable spawnTable,
        PieceSpawnModifiers spawnModifiers)
    {
        if (spawnTable.WeightedPieceTypes == null ||
            spawnTable.WeightedPieceTypes.Length == 0)
        {
            Debug.LogError("WeightedPieceTypesが設定されていません。", this);
            return null;
        }

        int totalWeight = 0;

        // WeightedPieceTypeの重みを合計する
        foreach (var weightedPieceType in spawnTable.WeightedPieceTypes)
        {
            if (weightedPieceType == null
                || weightedPieceType.SpawnWeight <= 0)
            {
                continue;
            }

            if (!HasAvailablePieces(spawnTable.CastlePartType, weightedPieceType.PieceType))
            {
                continue;
            }

            totalWeight += CalculateTypeWeight(
                weightedPieceType,
                spawnModifiers);
        }

        if (totalWeight <= 0)
        {
            Debug.LogError(
                "抽選可能なピースがありません。" +
                "SpawnWeightまたはスキルの補正値を確認してください。",
                this);
            return null;
        }

        // WeightedPieceTypeの重みに応じてランダムにPieceTypeを選択する
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (var weightedPieceType in spawnTable.WeightedPieceTypes)
        {
            if (weightedPieceType == null
                || weightedPieceType.SpawnWeight <= 0)
            {
                continue;
            }

            if (!HasAvailablePieces(spawnTable.CastlePartType, weightedPieceType.PieceType))
            {
                continue;
            }

            int spawnWeight = CalculateTypeWeight(
                weightedPieceType,
                spawnModifiers);

            if (spawnWeight <= 0)
            {
                continue;
            }

            if (randomValue < spawnWeight)
            {
                spawnModifiers?.ConsumeDraw();
                return weightedPieceType.PieceType;
            }

            randomValue -= spawnWeight;
        }

        Debug.LogError("WeightedPieceTypeの選択に失敗しました。", this);
        return null;
    }

    /// <summary>
    ///     CastlePartTypeとPieceTypeに応じたピース定義を重みに応じてランダムに選択する
    /// </summary>
    /// <param name="castlePartType">キャッスルパートの種類</param>
    /// <param name="pieceType">ピースの種類</param>
    /// <returns>選択されたピース定義</returns>
    private PieceDefinition SelectPieceByWeight(
        CastlePartType castlePartType,
        PieceType pieceType)
    {
        // キャラクター定義の追加ピースセットとステージ設定の共通ピースセットを取得する
        PieceSet commonPieceSet = _stageSettings.CommonPieceSet;

        PieceSet characterPieceSet = _characterDefinition != null
            ? _characterDefinition.AdditionalPieceSet
            : null;

        // 共通ピースセットとキャラクター定義の追加ピースセットの重みを合計する
        int totalWeight = CalculatePieceSetWeight(
            commonPieceSet,
            castlePartType,
            pieceType);

        totalWeight += CalculatePieceSetWeight(
            characterPieceSet,
            castlePartType,
            pieceType);

        if (totalWeight <= 0)
        {
            Debug.LogError($"{castlePartType}の{pieceType}に対応するピースが設定されていません ", this);
            return null;
        }

        // 重みに応じてランダムにWeightedPieceを選択する
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        PieceDefinition pieceDefinition = SelectPieceFromSet(
            commonPieceSet,
            castlePartType,
            pieceType,
            ref randomValue);

        if (pieceDefinition != null)
        {
            return pieceDefinition;
        }

        return SelectPieceFromSet(
            characterPieceSet,
            castlePartType,
            pieceType,
            ref randomValue);
    }

    /// <summary>
    ///     全体物理設定を取得する
    /// </summary>
    /// <returns>全体物理設定</returns>
    private GlobalPiecePhysicsSettings GetGlobalPiecePhysicsSettings()
    {
        if (!_stageSettings.UseGlobalPiecePhysicsSettings)
        {
            return null;
        }

        if (_stageSettings.GlobalPiecePhysicsSettings != null)
        {
            return _stageSettings.GlobalPiecePhysicsSettings;
        }

        Debug.LogWarning(
            "全体物理設定を使用する設定ですが、" +
            "GlobalPiecePhysicsSettingsが設定されていません。" +
            "PieceDefinition個別の物理設定を使用します。",
            this);

        return null;
    }

    /// <summary>
    ///     CastlePartTypeとPieceTypeに応じたピースが存在するかどうかを判定する
    /// </summary>
    /// <param name="castlePartType">キャッスルパートの種類</param>
    /// <param name="pieceType">ピースの種類</param>
    /// <returns>ピースが存在するかどうか</returns>
    private bool HasAvailablePieces(CastlePartType castlePartType, PieceType pieceType)
    {
        if (CalculatePieceSetWeight(
            _stageSettings.CommonPieceSet,
            castlePartType,
            pieceType) > 0)
        {
            return true;
        }

        if (_characterDefinition == null)
        {
            return false;
        }

        return CalculatePieceSetWeight(
            _characterDefinition.AdditionalPieceSet,
            castlePartType,
            pieceType) > 0;
    }

    /// <summary>
    ///     PieceSpawnerの初期化処理を行う
    /// </summary>
    /// <returns>設定が有効かどうか</returns>
    private bool ValidateSettings()
    {
        if (_spawnPoint == null)
        {
            Debug.LogError("SpawnPointが設定されていません。", this);
            return false;
        }

        if (_defaultPiecePrefab == null)
        {
            Debug.LogError("DefaultPiecePrefabが設定されていません。", this);
            return false;
        }

        if (_stageSettings == null)
        {
            Debug.LogError("StageSettingsが設定されていません。", this);
            return false;
        }

        if (_stageSettings.HeightSpawnTables == null ||
            _stageSettings.HeightSpawnTables.Length == 0)
        {
            Debug.LogError("HeightSpawnTablesが設定されていません。", this);
            return false;
        }

        return true;
    }

    /// <summary>
    ///     スキルの補正を適用したピースの重みを計算する
    /// </summary>
    /// <param name="weightedPieceType">重み付きのピース設定</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>補正後の重み</returns>
    private static int CalculateTypeWeight(WeightedPieceType weightedPieceType, PieceSpawnModifiers spawnModifiers)
    {
        if (spawnModifiers == null)
        {
            return weightedPieceType.SpawnWeight;
        }

        return spawnModifiers.CalculateWeight(
            weightedPieceType.PieceType,
            weightedPieceType.SpawnWeight);
    }

    /// <summary>
    ///     PieceSetの中から、指定されたCastlePartTypeとPieceTypeに対応するWeightedPieceの重みを合計する
    /// </summary>
    /// <param name="pieceSet">ピースセット</param>
    /// <param name="castlePartType">キャッスルパートの種類</param>
    /// <param name="pieceType">ピースの種類</param>
    /// <returns>合計された重み</returns>
    private static int CalculatePieceSetWeight(
        PieceSet pieceSet,
        CastlePartType castlePartType,
        PieceType pieceType)
    {
        if (pieceSet == null ||
            pieceSet.WeightedPieces == null)
        {
            return 0;
        }

        int totalWeight = 0;

        // WeightedPieceの重みを合計する
        foreach (var piece in pieceSet.WeightedPieces)
        {
            if (!IsValidWeightedPiece(piece, castlePartType, pieceType))
            {
                continue;
            }

            totalWeight += piece.SpawnWeight;
        }

        return totalWeight;
    }

    /// <summary>
    ///     PieceSetの中から、指定されたCastlePartTypeとPieceTypeに対応するWeightedPieceを重みに応じてランダムに選択する
    /// </summary>
    /// <param name="pieceSet">ピースセット</param>
    /// <param name="castlePartType">キャッスルパートの種類</param>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="randomValue">ランダム値</param>
    /// <returns>選択されたピース定義</returns>
    private static PieceDefinition SelectPieceFromSet(
        PieceSet pieceSet,
        CastlePartType castlePartType,
        PieceType pieceType,
        ref int randomValue)
    {
        if (pieceSet == null ||
            pieceSet.WeightedPieces == null)
        {
            return null;
        }

        foreach (var weightedPiece in pieceSet.WeightedPieces)
        {
            if (!IsValidWeightedPiece(weightedPiece, castlePartType, pieceType))
            {
                continue;
            }

            if (randomValue < weightedPiece.SpawnWeight)
            {
                return weightedPiece.PieceDefinition;
            }

            randomValue -= weightedPiece.SpawnWeight;
        }
        return null;
    }

    /// <summary>
    ///     WeightedPieceの設定が有効かどうかを検証する
    /// </summary>
    /// <param name="weightedPiece">重み付きのピース設定</param>
    /// <param name="castlePartType">城のパートタイプ</param>
    /// <param name="pieceType">ピースタイプ</param>
    /// <returns>設定が有効かどうか</returns>
    private static bool IsValidWeightedPiece(
        WeightedPiece weightedPiece,
        CastlePartType castlePartType,
        PieceType pieceType)
    {
        if (weightedPiece == null ||
            weightedPiece.PieceDefinition == null ||
            weightedPiece.SpawnWeight <= 0)
        {
            return false;
        }

        PieceDefinition pieceDefinition =
            weightedPiece.PieceDefinition;

        if (pieceDefinition.Sprite == null)
        {
            return false;
        }

        if (pieceDefinition.PieceType != pieceType)
        {
            return false;
        }

        // 妨害ピースは城の部位に関係なく抽選対象にする
        if (pieceType == PieceType.Obstacle)
        {
            return true;
        }

        return pieceDefinition.PartType ==
            castlePartType;
    }
}