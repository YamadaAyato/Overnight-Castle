using UnityEngine;

/// <summary>
///     ピースの種類ごとのスポーン重み補正を保持するクラス
/// </summary>
public class PieceSpawnModifiers
{
    /// <summary> 普通のピースのスポーン重み補正 </summary>
    public float NormalWeightMultiplier { get; private set; } = 1f;

    /// <summary> ボーナスピースのスポーン重み補正 </summary>
    public float BonusWeightMultiplier { get; private set; } = 1f;

    /// <summary> 障害物ピースのスポーン重み補正 </summary>
    public float ObstacleWeightMultiplier { get; private set; } = 1f;

    /// <summary>
    ///     指定したピースの種類の重み補正を設定する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="modifier">重み補正値</param>
    public void SetWeightMultiplier(PieceType pieceType, float modifier)
    {
        float validModifier = Mathf.Max(0f, modifier);

        switch (pieceType)
        {
            case PieceType.Normal:
                NormalWeightMultiplier = validModifier;
                break;
            case PieceType.Bonus:
                BonusWeightMultiplier = validModifier;
                break;
            case PieceType.Obstacle:
                ObstacleWeightMultiplier = validModifier;
                break;
        }
    }

    /// <summary>
    ///     指定したピースの種類の重み補正をリセットする
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    public void ResetWeightMultiplier(PieceType pieceType)
    {
        SetWeightMultiplier(pieceType, 1f);
    }

    /// <summary>
    ///     全ての重み補正をリセットする
    /// </summary>
    public void ResetAllWeightMultipliers()
    {
        NormalWeightMultiplier = 1f;
        BonusWeightMultiplier = 1f;
        ObstacleWeightMultiplier = 1f;
    }

    /// <summary>
    ///     ピースの種類に応じて補正後の重みを計算する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="baseWeight">基準となる重み</param>
    /// <returns>補正後の重み</returns>
    public int CalculateWeight(PieceType pieceType, int baseWeight)
    {
        if (baseWeight <= 0)
        {
            return 0;
        }

        float multiplier = pieceType switch
        {
            PieceType.Normal => NormalWeightMultiplier,
            PieceType.Bonus => BonusWeightMultiplier,
            PieceType.Obstacle => ObstacleWeightMultiplier,
            _ => 1f
        };

        return Mathf.Max(0, Mathf.RoundToInt(baseWeight * multiplier));
    }
}
