using UnityEngine;

/// <summary>
/// スコアを管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>現在のキャッスルスコアの結果</summary>
    public CastleScoreResult CastleScoreResult { get; private set; }

    /// <summary>最高スコア</summary>
    public int MaxScore { get; private set; } = 0;

    [SerializeField] private ScoreSender _scoreSender;
    [SerializeField] private ScoreUI _scoreUI;

    public void SetCastleScoreResult(CastleScoreResult castleScoreResult)
    {
        CastleScoreResult = castleScoreResult;

        if (CastleScoreResult.TotalScore > MaxScore)
        {
            MaxScore = CastleScoreResult.TotalScore;
            _scoreUI.SetUI(CastleScoreResult);
        }
    }

    /// <summary>
    /// スコアをリセット
    /// </summary>
    public void ResetScore()
    {
        CastleScoreResult = default;
    }

    /// <summary>
    /// 最高スコアを送信する
    /// </summary>
    public void SendMaxScore()
    {
        _scoreSender.SendHighScore(MaxScore);
    }
}