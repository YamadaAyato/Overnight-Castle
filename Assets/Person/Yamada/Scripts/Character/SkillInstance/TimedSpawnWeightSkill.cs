using System;
using UnityEngine;

/// <summary>
///     指定したピースの生成される重みを一定時間補正するスキル効果
/// </summary>
[Serializable]
public class TimedSpawnWeightSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        context.AddTimedWeightMultiplier(_pieceType, _spawnWeightMultiplier, _duration);
    }

    [SerializeField, Tooltip("補正するピースの種類")]
    private PieceType _pieceType;

    [SerializeField, Tooltip("生成されるピースの重みの倍率")]
    private float _spawnWeightMultiplier;

    [SerializeField, Min(0f), Tooltip("補正を適用する時間(秒)")]
    private float _duration;
}
