using System;
using UnityEngine;

[Serializable]
public class DropCountSpawnWeightSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        
    }

    [SerializeField,Tooltip("補正するピースの種類")] 
    private PieceType _pieceType;

    [SerializeField,Tooltip("生成されるピースの重みの倍率")] 
    private float _spawnWeightMultiplier;

    [SerializeField,Tooltip("補正を適用するピースの数")] 
    private int _drawCount;
}
