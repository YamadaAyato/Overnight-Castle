using UnityEngine;

/// <summary>
/// スコアを管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>現在のスコア</summary>
    public int CurrentScore { get; private set; }
    /// <summary>最高スコア</summary>
    public int MaxScore { get; private set; }

    [SerializeField]
    private ScoreSender _scoreSender;

    /// <summary>
    /// スコアを加算
    /// </summary>
    public void AddScore(int value)
    {
        CurrentScore += value;

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
        CurrentScore = 0;
    }

    /// <summary>
    /// 最高スコアを送信する
    /// </summary>
    public void SendMaxScore()
    {
        _scoreSender.SendHighScore(MaxScore);
    }
}