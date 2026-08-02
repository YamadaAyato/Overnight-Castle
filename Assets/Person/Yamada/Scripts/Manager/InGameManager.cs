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
        if (_isTimeUp ||
            _isGameFinished)
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

    /// <summary>
    ///     指定されたピースタイプのピースを削除する
    /// </summary>
    /// <param name="targetPieceType">削除対象のピースタイプ</param>
    /// <param name="targetMode">削除モード</param>
    /// <param name="targetCount">削除するピースの数</param>
    public void RemovedPieces(
        PieceType targetPieceType,
        BoardPieceTargetMode targetMode,
        int targetCount)
    {
        List<FallingPiece> targets = GetSkillTargets(targetPieceType);
        int removeCount = GetEffectTargetCount(
            targets.Count,
            targetMode,
            targetCount);

        if (targetMode == BoardPieceTargetMode.SpecifiedCount)
        {
            ShufflePieces(targets);
        }

        for (int i = 0; i < removeCount; i++)
        {
            FallingPiece piece = targets[i];
            if (piece != null)
            {
                _spawnedPieces.Remove(piece);
                Destroy(piece.gameObject);
            }
        }
    }

    /// <summary>
    ///     障害物ピースを指定されたピースタイプに変換する
    /// </summary>
    /// <param name="targetPieceType">変換後のピースタイプ</param>
    /// <param name="targetMode">変換モード</param>
    /// <param name="targetCount">変換するピースの数</param>
    public void ConvertObstaclePieces(
        PieceType targetPieceType,
        BoardPieceTargetMode targetMode,
        int targetCount)
    {
        if (targetPieceType == PieceType.Obstacle)
        {
            return;
        }

        List<FallingPiece> targets = GetSkillTargets(PieceType.Obstacle);
        int convertCount = GetEffectTargetCount(
            targets.Count,
            targetMode,
            targetCount);

        if (targetMode == BoardPieceTargetMode.SpecifiedCount)
        {
            ShufflePieces(targets);
        }

        for (int i = 0; i < convertCount; i++)
        {
            FallingPiece piece = targets[i];

            float height = Mathf.Max(0f, piece.transform.position.y - _stageSettings.GroundPosY);

            PieceDefinition convertedDefinition =
                _spawner.GetRandomPieceDefinition(height, targetPieceType);

            if (convertedDefinition == null)
            {
                continue;
            }

            FallingPiece convertedPiece =
                _spawner.CreatePiece(convertedDefinition, piece.transform.position, piece.transform.rotation);

            if (convertedPiece == null)
            {
                continue;
            }

            _spawnedPieces.Remove(piece);
            piece.gameObject.SetActive(false);
            Destroy(piece.gameObject);

            convertedPiece.Drop();
            _spawnedPieces.Add(convertedPiece);
        }
    }

    [SerializeField] private PieceController _controller;
    [SerializeField] private PieceSpawner _spawner;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private StageSettings _stageSettings;
    [SerializeField] private CharacterSkillController _characterSkillController;
    [SerializeField] private CharacterImageManager _characterImageSpawner;
    [SerializeField] private CharacterDefinition _characterDefinition;
    [SerializeField] private DayCycleBackgroundView _dayCycleBackgroundView;
    [SerializeField] private DayNightCycle _dayNightCycle;
    [SerializeField] private CutInAnimation _cutIn;
    [SerializeField, Min(0f)] private float _stopLinearVelocityThreshold = 0.05f;
    [SerializeField, Min(0f)] private float _stopAngularVelocityThreshold = 2f;
    [SerializeField, Min(0f)] private float _requiredStopDuration = 0.5f;
    [SerializeField, Min(0f)] private float _forceGameEndTime = 5f;

    private readonly List<FallingPiece> _spawnedPieces = new List<FallingPiece>();
    private readonly PieceSpawnModifiers _modifiers = new();

    private bool _isWaitingForNextPiece;
    private bool _isTimeUp;
    private bool _isGameFinished;

    private void Start()
    {
        Debug.Log("ゲーム開始。", this);

        CharacterDefinition selectedCharacter = GetCharacterDefinition();

        if (selectedCharacter == null)
        {
            Debug.LogError("キャラクター定義が設定されていません。", this);
            enabled = false;
            return;
        }

        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        // Spawnerの初期化を行う
        if (!_spawner.Initialize(_stageSettings, selectedCharacter))
        {
            enabled = false;
            return;
        }

        if (!_characterSkillController.Initialize(selectedCharacter, _modifiers, Timer.Instance, this))
        {
            enabled = false;
            return;
        }

        if(!_cutIn.Initialize(_characterSkillController,
            selectedCharacter.CharHead, 
            selectedCharacter.CharBody))
        {
            enabled = false;
            return;
        }

        if (_characterImageSpawner != null)
        {
            _characterImageSpawner.Spawn(selectedCharacter);
        }

        _controller.OnPieceDropped += HandlePieceDropped;
        Timer.Instance.OnTimeUp += HandleTimeUp;

        _controller.SetStageWidth(_stageSettings.StageWidth);
        _controller.StartControl();
        _scoreManager.ResetScore();
        _modifiers.ResetAllWeightMultipliers();

        Timer.Instance.StartTimer(_stageSettings.StageTimeLimit);
        _dayCycleBackgroundView.StartScroll(Timer.Instance);
        _dayNightCycle.Initialize(Timer.Instance);

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
            Timer.Instance.OnTimeUp -= HandleTimeUp;
        }
    }

    /// <summary>
    ///     指定されたピースタイプのスキル対象となるピースを取得する
    /// </summary>
    /// <param name="pieceType">ピースタイプ</param>
    /// <returns>スキル対象となるピースのリスト</returns>
    private List<FallingPiece> GetSkillTargets(PieceType pieceType)
    {
        List<FallingPiece> targets = new List<FallingPiece>();

        foreach (var piece in _spawnedPieces)
        {
            if (piece == null ||
                !piece.HasDropped ||
                !piece.HasLanded ||
                piece.PieceType != pieceType)
            {
                continue;
            }

            if (!piece.IsStopped(
                _stopLinearVelocityThreshold,
                _stopAngularVelocityThreshold))
            {
                continue;
            }
            targets.Add(piece);
        }
        return targets;
    }

    /// <summary>
    ///     ピースが落下したときの処理
    /// </summary>
    /// <param name="piece">落下したピース</param>
    private void HandlePieceDropped(FallingPiece piece)
    {
        if (_isGameFinished ||
            _isWaitingForNextPiece ||
            _isTimeUp)
        {
            return;
        }

        _ = SpawnNextPieceAsync();
    }

    /// <summary>
    ///     タイムアップ時の処理
    /// </summary>
    private void HandleTimeUp()
    {
        if (_isTimeUp ||
            _isGameFinished)
        {
            return;
        }

        _isTimeUp = true;

        _characterSkillController.StopAllSkills();
        _modifiers.ResetAllWeightMultipliers();

        // タイムアップ時は、ピースの操作を停止し、落下中のピースが停止するまで待機する
        _controller.StopControl();
        _ = WaitForDroppedPiecesToStopAsync();
    }

    /// <summary>
    ///     落下中のピースがすべて停止しているかどうかを判定する
    /// </summary>
    /// <returns></returns>
    private bool AreAllPiecesStopped()
    {
        foreach (var piece in _spawnedPieces)
        {
            if (piece == null ||
                !piece.HasDropped)
            {
                continue;
            }

            if (!piece.IsStopped(
                _stopLinearVelocityThreshold,
                _stopAngularVelocityThreshold))
            {
                return false;
            }

            BombPiece bombPiece = piece.GetComponent<BombPiece>();

            if (bombPiece != null &&
                bombPiece.IsWaitingToExplode)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     落下中のピースがすべて停止するまで待機する
    /// </summary>
    /// <returns></returns>
    private async Awaitable WaitForDroppedPiecesToStopAsync()
    {
        float stoppedDuration = 0f;
        float elapsedTime = 0f;

        try
        {
            while (stoppedDuration < _requiredStopDuration &&
                elapsedTime < _forceGameEndTime)
            {
                if (AreAllPiecesStopped())
                {
                    stoppedDuration += Time.deltaTime;
                }
                else
                {
                    stoppedDuration = 0f;
                }

                elapsedTime += Time.deltaTime;

                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }

        FinishGame();
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
        if (_isGameFinished ||
            _isTimeUp)
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
            if (piece == null || !piece.HasLanded)
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
    ///     現在のキャラクター定義を取得する
    /// </summary>
    /// <returns>現在のキャラクター定義</returns>
    private CharacterDefinition GetCharacterDefinition()
    {
        if (GameSession.TryGetSelectedCharacter(out CharacterDefinition character))
        {
            return character;
        }

        // このシーンで選択されたキャラクターがない場合は、デフォルトのキャラクター定義を返す
        return _characterDefinition;
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
        if (_scoreManager == null)
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

    /// <summary>
    ///     指定されたターゲットモードに基づいて、効果の対象となるピースの数を取得する
    /// </summary>
    /// <param name="availableCount">利用可能なピースの数</param>
    /// <param name="targetMode">ターゲットモード</param>
    /// <param name="specifiedCount">指定されたピースの数</param>
    /// <returns>効果の対象となるピースの数</returns>
    private static int GetEffectTargetCount(
        int availableCount,
        BoardPieceTargetMode targetMode,
        int specifiedCount)
    {
        if (targetMode == BoardPieceTargetMode.All)
        {
            return availableCount;
        }

        return Mathf.Min(availableCount, Mathf.Max(0, specifiedCount));
    }

    /// <summary>
    ///     ピースのリストをシャッフルする
    /// </summary>
    /// <param name="pieces">シャッフルするピースのリスト</param>
    private static void ShufflePieces(List<FallingPiece> pieces)
    {
        for (int i = pieces.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            FallingPiece temp = pieces[i];
            pieces[i] = pieces[randomIndex];
            pieces[randomIndex] = temp;
        }
    }
}
