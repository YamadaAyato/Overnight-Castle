/// <summary>
///     城のスコア結果を保持する構造体
/// </summary>
public readonly struct CastleScoreResult
{
    public CastleScoreResult(float height, int heightScore, int completionScore, int totalScore)
    {
        Height = height;
        HeightScore = heightScore;
        CompletionScore = completionScore;
        TotalScore = totalScore;
    }

    /// <summary> 城の高さ </summary>
    public float Height { get; }

    /// <summary> 高さに応じたスコア </summary>
    public int HeightScore { get; }

    /// <summary> 完成度に応じたスコア </summary>
    public int CompletionScore { get; }

    /// <summary> 合計スコア </summary>
    public int TotalScore { get; }
}
