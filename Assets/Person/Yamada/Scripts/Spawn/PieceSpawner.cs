using UnityEngine;

/// <summary>
///     ピースの抽選と生成を管理するクラス
/// </summary>
public class PieceSpawner : MonoBehaviour
{
    /// <summary>
    ///     ピースの生成処理に必要な設定を初期化する
    /// </summary>
    /// <param name="stageSettings">ステージ設定</param>
    /// <returns>初期化に成功したかどうか</returns>
    public bool Initialize(StageSettings stageSettings)
    {
        _stageSettings = stageSettings;

        return ValidateSettings();
    }

    /// <summary>
    ///     現在の高さと抽選補正に応じたピースを生成する
    /// </summary>
    /// <param name="currentHeight">現在の高さ</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>生成されたピース</returns>
    public FallingPiece SpawnPiece(float currentHeight, PieceSpawnModifiers spawnModifiers)
    {
        if (_stageSettings == null)
        {
            Debug.LogError("PieceSpawnerが初期化されていません。", this);
            return null;
        }

        HeightSpawnTable spawnTable = GetSpawnTable(currentHeight);

        FallingPiece prefab = SelectPieceByWeight(
                spawnTable,
                spawnModifiers);

        if (prefab == null)
        {
            Debug.LogError($"高さ{currentHeight:F1}に対応するピースを生成できませんでした。", this);
            return null;
        }

        FallingPiece piece = Instantiate(
            prefab,
            _spawnPoint.position,
            Quaternion.identity);

        GlobalPiecePhysicsSettings globalPiecePhysicsSettings = GetGlobalPiecePhysicsSettings();

        piece.Initialize(
            globalPiecePhysicsSettings,
            _stageSettings.DeletePositionY);

        return piece;
    }

    [SerializeField] private Transform _spawnPoint;

    private StageSettings _stageSettings;

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
    ///     WeightedPieceの重みに応じて、FallingPieceを選択する
    /// </summary>
    /// <param name="spawnTable">高さに応じたSpawnTable</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>選択されたFallingPiece</returns>
    private FallingPiece SelectPieceByWeight(
        HeightSpawnTable spawnTable,
        PieceSpawnModifiers spawnModifiers)
    {
        if (spawnTable == null ||
            spawnTable.WeightedPieces == null ||
            spawnTable.WeightedPieces.Length == 0)
        {
            Debug.LogError("WeightedPiecesが設定されていません。", this);
            return null;
        }

        int totalWeight = 0;

        // WeightedPieceの設定が有効かどうかを検証する
        foreach (var weightedPiece in spawnTable.WeightedPieces)
        {
            if (!IsValidWeightedPiece(weightedPiece))
            {
                return null;
            }

            totalWeight += CalculateSpawnWeight(
                weightedPiece,
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

        // 重みに応じてランダムにWeightedPieceを選択する
        int randomValue = Random.Range(0, totalWeight);

        // 選択されたWeightedPieceに対応するFallingPieceを返す
        foreach (var weightedPiece in spawnTable.WeightedPieces)
        {
            int spawnWeight = CalculateSpawnWeight(
                weightedPiece,
                spawnModifiers);

            // 補正後の重みが0以下の場合は抽選対象から除外する
            if (spawnWeight <= 0)
            {
                continue;
            }

            if (randomValue < spawnWeight)
            {
                return weightedPiece.PiecePrefab;
            }

            // 重みを減算して次のWeightedPieceの範囲に移動する
            randomValue -= spawnWeight;
        }

        Debug.LogError("WeightedPieceの選択に失敗しました。", this);
        return null;
    }

    /// <summary>
    ///     スキルの補正を適用したピースの重みを計算する
    /// </summary>
    /// <param name="weightedPiece">重み付きのピース設定</param>
    /// <param name="spawnModifiers">ピース抽選の補正設定</param>
    /// <returns>補正後の重み</returns>
    private static int CalculateSpawnWeight(WeightedPiece weightedPiece, PieceSpawnModifiers spawnModifiers)
    {
        if (spawnModifiers == null)
        {
            return weightedPiece.SpawnWeight;
        }

        return spawnModifiers.CalculateWeight(
            weightedPiece.PiecePrefab.PieceType,
            weightedPiece.SpawnWeight);
    }

    /// <summary>
    ///     全体物理設定を取得する
    /// </summary>
    /// <returns>全体物理設定</returns>
    private GlobalPiecePhysicsSettings
        GetGlobalPiecePhysicsSettings()
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
            "Prefab個別の物理設定を使用します。",
            this);

        return null;
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
    ///     WeightedPieceの設定が有効かどうかを検証する
    /// </summary>
    /// <param name="weightedPiece">重み付きのピース設定</param>
    /// <returns>設定が有効かどうか</returns>
    private static bool IsValidWeightedPiece(WeightedPiece weightedPiece)
    {
        if (weightedPiece == null)
        {
            Debug.LogError("WeightedPieceがnullです。");
            return false;
        }

        if (weightedPiece.PiecePrefab == null)
        {
            Debug.LogError("WeightedPieceのPiecePrefabが設定されていません。");
            return false;
        }

        if (weightedPiece.SpawnWeight <= 0)
        {
            Debug.LogError("WeightedPieceのWeightが0以下です。");
            return false;
        }

        return true;
    }
}