using System;
using UnityEngine;

/// <summary>
///     妨害ピースを、指定された種類のピースに変換するスキル効果クラス
/// </summary>
[Serializable]
public class ConvertObstaclePiecesSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        context.ConvertObstaclePieces(_targetPieceType, _obstaclePieceTargetMode, _count);
    }

    [SerializeField, Tooltip("変換後ピースの種類")]
    private PieceType _targetPieceType;

    [SerializeField, Tooltip("変換するピースのモード、Allですべてのピースを変換か指定数削除モード")]
    private BoardPieceTargetMode _obstaclePieceTargetMode;

    [SerializeField, Min(1f), Tooltip("変換するピースの数")]
    private int _count;
}
