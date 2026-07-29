using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     ピースの種類ごとのスポーン重み補正を保持するクラス
/// </summary>
public class PieceSpawnModifiers
{
    /// <summary> 普通のピースのスポーン重み補正 </summary>
    public float NormalWeightMultiplier => CalculateMultiplier(PieceType.Normal);

    /// <summary> ボーナスピースのスポーン重み補正 </summary>
    public float BonusWeightMultiplier => CalculateMultiplier(PieceType.Bonus);

    /// <summary> 障害物ピースのスポーン重み補正 </summary>
    public float ObstacleWeightMultiplier => CalculateMultiplier(PieceType.Obstacle);

    /// <summary>
    ///     指定したピースの種類の重み補正を設定する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="modifier">重み補正値</param>
    public void SetWeightMultiplier(PieceType pieceType, float modifier)
    {
        ResetWeightMultiplier(pieceType);

        float validModifier = Mathf.Max(0f, modifier);

        if (Mathf.Approximately(validModifier, 1f))
        {
            return;
        }

        AddWeightMultiplier(pieceType, validModifier);
    }

    /// <summary>
    ///     指定したピースの種類の重み補正を追加する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="modifier">重み補正値</param>
    /// <returns>追加した補正のID</returns>
    public int AddWeightMultiplier(PieceType pieceType, float modifier)
    {
        int modifierId = _nextModifierId++;

        _weightModifiers.Add(new WeightModifier
        {
            Id = modifierId,
            PieceType = pieceType,
            Multiplier = Mathf.Max(0f, modifier)
        });

        return modifierId;
    }

    /// <summary>
    ///     指定した回数だけ有効な重み補正を追加する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <param name="modifier">重み補正値</param>
    /// <param name="drawCount">補正を適用する抽選回数</param>
    /// <returns>追加した補正のID</returns>
    public int AddDrawCountWeightMultiplier(PieceType pieceType, float modifier, int drawCount)
    {
        if (drawCount <= 0)
        {
            return 0;
        }

        int modifierId = _nextModifierId++;

        _weightModifiers.Add(new WeightModifier
        {
            Id = modifierId,
            PieceType = pieceType,
            Multiplier = Mathf.Max(0f, modifier),
            RemainingDrawCount = drawCount,
            UsesDrawCount = true
        });

        return modifierId;
    }

    /// <summary>
    ///     指定したIDの重み補正を削除する
    /// </summary>
    /// <param name="modifierId">削除する補正のID</param>
    /// <returns>削除できた場合はtrue、それ以外はfalse</returns>
    public bool RemoveWeightMultiplier(int modifierId)
    {
        // 後ろから削除することで、インデックスのずれを防ぐ
        for (int index = _weightModifiers.Count - 1; index >= 0; index--)
        {
            if (_weightModifiers[index].Id != modifierId)
            {
                continue;
            }

            _weightModifiers.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     抽選回数を1回消費する
    /// </summary>
    public void ConsumeDraw()
    {
        for (int index = _weightModifiers.Count - 1; index >= 0; index--)
        {
            WeightModifier modifier = _weightModifiers[index];

            if (!modifier.UsesDrawCount)
            {
                continue;
            }

            modifier.RemainingDrawCount--;

            if (modifier.RemainingDrawCount <= 0)
            {
                _weightModifiers.RemoveAt(index);
            }
        }
    }

    /// <summary>
    ///     指定したピースの種類の重み補正をリセットする
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    public void ResetWeightMultiplier(PieceType pieceType)
    {
        // 後ろから削除することで、インデックスのずれを防ぐ
        for (int index = _weightModifiers.Count - 1; index >= 0; index--)
        {
            if (_weightModifiers[index].PieceType != pieceType)
            {
                continue;
            }

            _weightModifiers.RemoveAt(index);
        }
    }

    /// <summary>
    ///     全ての重み補正をリセットする
    /// </summary>
    public void ResetAllWeightMultipliers()
    {
        _weightModifiers.Clear();
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

        float multiplier = CalculateMultiplier(pieceType);

        return Mathf.Max(0, Mathf.RoundToInt(baseWeight * multiplier));
    }

    private readonly List<WeightModifier> _weightModifiers = new List<WeightModifier>();
    private int _nextModifierId = 1;

    /// <summary>
    ///     指定したピースの種類に適用される重み補正を計算する
    /// </summary>
    /// <param name="pieceType">ピースの種類</param>
    /// <returns>適用される重み補正</returns>
    private float CalculateMultiplier(PieceType pieceType)
    {
        float multiplier = 1f;

        foreach (var weightModifier in _weightModifiers)
        {
            if (weightModifier.PieceType != pieceType)
            {
                continue;
            }

            multiplier *= weightModifier.Multiplier;
        }

        return Mathf.Max(0f, multiplier);
    }

    /// <summary>
    ///     重み補正の情報を保持するクラス
    /// </summary>
    private class WeightModifier
    {
        public int Id;
        public PieceType PieceType;
        public float Multiplier;
        public int RemainingDrawCount;
        public bool UsesDrawCount;
    }
}