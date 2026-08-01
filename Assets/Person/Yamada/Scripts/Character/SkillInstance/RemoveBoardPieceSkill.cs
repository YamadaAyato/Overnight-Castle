using System;
using UnityEngine;

/// <summary>
///     盤面上のピースを削除するスキル効果クラス
/// </summary>
[Serializable]
public class RemoveBoardPieceSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        context.RemoveBoardPieces(_targetPieceType, _boardPieceTargetMode, _count);
    }

    [SerializeField, Tooltip("削除するピースの種類")]
    private PieceType _targetPieceType;

    [SerializeField, Tooltip("削除するピースのモード、Allですべてのピースを削除か指定数削除モード")]
    private BoardPieceTargetMode _boardPieceTargetMode;

    [SerializeField, Min(1f), Tooltip("削除するピースの数")]
    private int _count;
}
