using DG.Tweening;
using UnityEngine;

/// <summary>
/// スコアを管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>現在のキャッスルスコアの結果</summary>
    public CastleScoreResult ScoreResult { get; private set; }

    /// <summary>最高スコア</summary>
    public int MaxScore { get; private set; } = 0;

    [SerializeField] private ScoreSender _scoreSender;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private CameraAnimation _cameraAnimation;
    [SerializeField] private TargetCamera _targetCamera;

    public void SetCastleScoreResult(CastleScoreResult castleScoreResult)
    {
        _targetCamera.SetTrackingEnabled(false);
        ScoreResult = castleScoreResult;
        bool isMax = false;

        if (ScoreResult.TotalScore > MaxScore)
        {
            isMax = true;
        }

        _cameraAnimation.PlayCameraMove()
        .OnComplete(() =>
        {
            _scoreUI.SetUI(ScoreResult, isMax);
        });
    }

    /// <summary>
    /// スコアをリセット
    /// </summary>
    public void ResetScore()
    {
        ScoreResult = default;
    }

    /// <summary>
    /// 最高スコアを送信する
    /// </summary>
    public void SendMaxScore()
    {
        _scoreSender.SendHighScore(MaxScore);
    }
}