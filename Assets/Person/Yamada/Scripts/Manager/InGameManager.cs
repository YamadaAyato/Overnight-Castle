using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public bool IsGameFinished => _isGameFinished;

    public void SpawnPiece()
    {
        if (_isGameFinished)
        {
            return;
        }

        float currentHeight = CalculateCurrentHeight();

        HeightSpawnTable spawnTable = GetSpawnTable(currentHeight);
        FallingPiece prefab = SelectPieceByWeight(spawnTable);

        if (prefab == null)
        {
            Debug.LogError(
                $"高さ{currentHeight:F1}に対応するピースを生成できませんでした。",
                this);

            return;
        }

        FallingPiece piece = Instantiate(prefab, _spawnPoint.position, Quaternion.identity);
        GlobalPiecePhysicsSettings globalPiecePhysicsSettings = GetGlobalPiecePhysicsSettings();

        piece.Initialize(globalPiecePhysicsSettings, _stageSettings.DeletePositionY);

        _spawnedPieces.Add(piece);
        _controller.SetCurrentPiece(piece);
    }

    public void FinishGame()
    {
        if (_isGameFinished)
        {
            return;
        }

        _isGameFinished = true;
        _controller.StopControl();

        foreach (var piece in _spawnedPieces)
        {
            if (piece != null)
            {
                piece.Fix();
            }
        }
    }

    [SerializeField] private PieceController _controller;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private StageSettings _stageSettings;

    private readonly List<FallingPiece> _spawnedPieces = new List<FallingPiece>();

    private bool _isWaitingForNextPiece;
    private bool _isGameFinished;

    private void Start()
    {
        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        _controller.OnPieceDropped += HandlePieceDropped;
        Timer.Instance.OnTimeUp += FinishGame;

        _controller.SetStageWidth(_stageSettings.StageWidth);
        Timer.Instance.StartTimer(_stageSettings.StageTimeLimit);
        SpawnPiece();
    }

    private void OnDestroy()
    {
        if (_controller != null)
        {
            _controller.OnPieceDropped -= HandlePieceDropped;
        }

        if (Timer.Instance != null)
        {
            Timer.Instance.OnTimeUp -= FinishGame;
        }
    }

    /// <summary>
    ///     ピースが落下したときの処理
    /// </summary>
    /// <param name="piece">落下したピース</param>
    private void HandlePieceDropped(FallingPiece piece)
    {
        if (_isGameFinished ||
            _isWaitingForNextPiece)
        {
            return;
        }

        _ = SpawnNextPieceAsync();
    }

    /// <summary>
    ///     次のピースを生成するまで待機する
    /// </summary>
    /// <returns></returns>
    private async Awaitable SpawnNextPieceAsync()
    {
        _isWaitingForNextPiece = true;

        try
        {
            await Awaitable.WaitForSecondsAsync(
                _stageSettings.NextSpawnDelay,
                destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            // GameManagerが破棄された場合、待機を中断する
            return;
        }
        finally
        {
            _isWaitingForNextPiece = false;
        }

        // ゲームが終了していない場合のみ、次のピースを生成する
        if (_isGameFinished)
        {
            return;
        }

        SpawnPiece();
    }

    /// <summary>
    ///     現在の高さを計算する
    /// </summary>
    /// <returns>現在の高さ</returns>
    private float CalculateCurrentHeight()
    {
        float highestPoitionY = _stageSettings.GroundPosY;

        foreach (var piece in _spawnedPieces)
        {
            if (piece == null || !piece.HasLanded)
            {
                continue;
            }

            highestPoitionY = Mathf.Max(highestPoitionY, piece.HighestPositionY);
        }

        return Mathf.Max(0f, highestPoitionY - _stageSettings.GroundPosY);
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
            if (table == null || height < table.HeightThreshold)
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

        // SpawnTableが見つからなかった場合はエラーを出力する
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
    /// <returns>選択されたFallingPiece</returns>
    private FallingPiece SelectPieceByWeight(HeightSpawnTable spawnTable)
    {
        if (spawnTable == null || spawnTable.WeightedPieces == null || spawnTable.WeightedPieces.Length == 0)
        {
            Debug.LogError("WeightedPiecesが設定されていません。");
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
            totalWeight += weightedPiece.SpawnWeight;
        }

        // 重みに応じてランダムにWeightedPieceを選択する
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        // 選択されたWeightedPieceに対応するFallingPieceを返す
        foreach (var weightedPiece in spawnTable.WeightedPieces)
        {
            if (randomValue < weightedPiece.SpawnWeight)
            {
                return weightedPiece.PiecePrefab;
            }

            // 重みを減算して次のWeightedPieceの範囲に移動する
            randomValue -= weightedPiece.SpawnWeight;
        }

        Debug.LogError("WeightedPieceの選択に失敗しました。");
        return null;
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
            "Prefab個別の物理設定を使用します。",
            this);

        return null;
    }

    /// <summary>
    ///     ゲームの初期化処理を行う
    /// </summary>
    /// <returns>設定が有効かどうか</returns>
    private bool ValidateSettings()
    {
        if (_controller == null)
        {
            Debug.LogError("PieceControllerが設定されていません。");
            return false;
        }
        if (_spawnPoint == null)
        {
            Debug.LogError("SpawnPointが設定されていません。");
            return false;
        }
        if (_stageSettings == null)
        {
            Debug.LogError("StageSettingsが設定されていません。");
            return false;
        }
        if (Timer.Instance == null)
        {
            Debug.LogError("Timerがシーンに存在しません。");
            return false;
        }
        if (_stageSettings.HeightSpawnTables == null || _stageSettings.HeightSpawnTables.Length == 0)
        {
            Debug.LogError("HeightSpawnTablesが設定されていません。");
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
