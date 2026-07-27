using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     城のスコアを計算するためのユーティリティクラス
/// </summary>
public static class CastleScoreCalculator
{
    /// <summary>
    ///     落下中のピースの中で最も高い位置を計算する
    /// </summary>
    /// <param name="fallingPieces"></param>
    /// <param name="groundPositionY"></param>
    /// <returns></returns>
    public static float CalculateHeight(
        IReadOnlyList<FallingPiece> fallingPieces,
        float groundPositionY)
    {
        float highestPositionY = groundPositionY;

        foreach (var piece in fallingPieces)
        {
            if (piece == null || !piece.HasLanded)
            {
                continue;
            }

            highestPositionY = Mathf.Max(highestPositionY, piece.HighestPositionY);
        }

        return Mathf.Max(0, highestPositionY - groundPositionY);
    }

    /// <summary>
    ///     スコアを計算する
    /// </summary>
    /// <param name="fallingPieces">落下中のピースのリスト</param>
    /// <param name="groundPositionY">地面のY座標</param>
    /// <param name="heightScoreMultiplier">高さスコアの倍率</param>
    /// <returns>計算結果のスコア</returns>
    public static CastleScoreResult CalculateScore(
        IReadOnlyList<FallingPiece> fallingPieces,
        float groundPositionY,
        float heightScoreMultiplier)
    {
        float height = CalculateHeight(fallingPieces, groundPositionY);
        int heightScore = Mathf.RoundToInt(height * heightScoreMultiplier);

        int completionScore = 0;

        // 落下中のピースの中で着地したピースのスコアを合計する
        foreach (var piece in fallingPieces)
        {
            if (piece == null || !piece.HasLanded)
            {
                continue;
            }

            completionScore += piece.Score;
        }

        // 総合スコアを計算する（高さスコアと完成スコアの合計）
        int totalScore = Mathf.Max(0, heightScore + completionScore);
        return new CastleScoreResult(height, heightScore, completionScore, totalScore);
    }
}
