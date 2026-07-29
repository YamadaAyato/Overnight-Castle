using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     ゲーム中の管理を行うクラス
/// </summary>
public class InGameManager : MonoBehaviour
{
    /// <summary> ゲームが終了しているかどうか </summary>
    public bool IsGameFinished => _isGameFinished;

    /// <summary> 現在の城の高さ </summary>
    public float CurrentHeight => CalculateCurrentHeight();

    /// <summary> ピースの抽選補正設定 </summary>
    public PieceSpawnModifiers Modifiers => _modifiers;

    /// <summary> ゲーム終了時の通知 </summary>
    public event Action OnGameFinished;

    /// <summary>
    ///     新しいピースを生成する
    /// </summary>
    public void SpawnPiece()
    {
        if (_isGameFinished)
        {
            return;
        }

        float currentHeight = CalculateCurrentHeight();

        // 現在の高さに応じた抽選テーブルを取得する
        FallingPiece piece = _spawner.SpawnPiece(
            currentHeight,
            _modifiers);

        if (piece == null)
        {
            Debug.LogError("ピースの生成に失敗しました。", this);
            return;
        }

        _spawnedPieces.Add(piece);
        _controller.SetCurrentPiece(piece);
    }

    /// <summary>
    ///     ゲームを終了する
    /// </summary>
    public void FinishGame()
    {
        if (_isGameFinished)
        {
            return;
        }

        _isGameFinished = true;
        _characterSkillController.StopAllSkills();
        _modifiers.ResetAllWeightMultipliers();
        _controller.StopControl();

        foreach (var piece in _spawnedPieces)
        {
            if (piece != null)
            {
                piece.Fix();
            }
        }

        // ゲーム終了時のスコアを計算する
        CastleScoreResult result = CastleScoreCalculator.CalculateScore(
            _spawnedPieces,
            _stageSettings.GroundPosY,
            _stageSettings.HeightScoreMultiplier);

        _scoreManager.SetCastleScoreResult(result);

        Debug.Log($"ゲーム終了: 高さ={result.Height}, " +
            $"高さスコア={result.HeightScore}, " +
            $"完成度スコア={result.CompletionScore}, " +
            $"合計スコア={result.TotalScore}");

        OnGameFinished?.Invoke();
    }

    [SerializeField] private PieceController _controller;
    [SerializeField] private PieceSpawner _spawner;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private StageSettings _stageSettings;
    [SerializeField] private CharacterSkillController _characterSkillController;
    [SerializeField] private CharacterDefinition _characterDefinition;

    private readonly List<FallingPiece> _spawnedPieces = new List<FallingPiece>();
    private readonly PieceSpawnModifiers _modifiers = new();

    private bool _isWaitingForNextPiece;
    private bool _isGameFinished;

    private void Start()
    {
        Debug.Log("ゲーム開始。", this);

        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        // Spawnerの初期化を行う
        if (!_spawner.Initialize(_stageSettings, _characterDefinition))
        {
            enabled = false;
            return;
        }

        if(!_characterSkillController.Initialize(_characterDefinition, _modifiers, Timer.Instance))
        {
            enabled = false;
            return;
        }

        _controller.OnPieceDropped += HandlePieceDropped;
        Timer.Instance.OnTimeUp += FinishGame;

        _controller.SetStageWidth(_stageSettings.StageWidth);
        _controller.StartControl();
        _scoreManager.ResetScore();
        _modifiers.ResetAllWeightMultipliers();

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
    ///     現在の城の高さを計算する
    /// </summary>
    /// <returns>現在の城の高さ</returns>
    private float CalculateCurrentHeight()
    {
        float highestPositionY = _stageSettings.GroundPosY;

        foreach (var piece in _spawnedPieces)
        {
            if (piece == null ||!piece.HasLanded)
            {
                continue;
            }

            highestPositionY = Mathf.Max(
                highestPositionY,
                piece.HighestPositionY);
        }

        return Mathf.Max(
            0f,
            highestPositionY -
            _stageSettings.GroundPosY);
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
        if (_spawner == null)
        {
            Debug.LogError("Spawnerが設定されていません。");
            return false;
        }
        if(_scoreManager == null)
        {
            Debug.LogError("ScoreManagerが設定されていません。");
            return false;
        }
        if (_characterSkillController == null)
        {
            Debug.LogError("CharacterSkillControllerが設定されていません。");
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
}
