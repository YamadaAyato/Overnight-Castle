using System;
using UnityEngine;

/// <summary>
///     指定された種類のピースの出現確率に補正を適用するスキル効果クラス
/// </summary>
[Serializable]
public class DropCountSpawnWeightSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        context.AddDrawCountWeightMultiplier(_pieceType, _spawnWeightMultiplier, _drawCount);
    }

    [SerializeField,Tooltip("補正するピースの種類")] 
    private PieceType _pieceType;

    [SerializeField,Tooltip("生成されるピースの重みの倍率")] 
    private float _spawnWeightMultiplier;

    [SerializeField,Tooltip("補正を適用するピースの数")] 
    private int _drawCount;
}
