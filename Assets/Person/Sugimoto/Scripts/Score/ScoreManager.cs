using UnityEngine;

/// <summary>
/// スコアを管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>現在のキャッスルスコアの結果</summary>
    public CastleScoreResult CastleScoreResult { get; private set; }

    /// <summary>現在のスコア</summary>
    public int CurrentScore => CastleScoreResult.TotalScore;

    /// <summary>最高スコア</summary>
    public int MaxScore { get; private set; }

    [SerializeField]
    private ScoreSender _scoreSender;

    public void SetCastleScoreResult(CastleScoreResult castleScoreResult)
    {
        CastleScoreResult = castleScoreResult;

        if (CurrentScore > MaxScore)
        {
            MaxScore = CurrentScore;
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