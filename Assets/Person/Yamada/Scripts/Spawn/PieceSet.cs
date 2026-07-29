using UnityEngine;

/// <summary>
///     抽選対象になるピースのセットを保持するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "PieceSet", menuName = "Overnight Castle/PieceSet")]
public class PieceSet : ScriptableObject
{
    /// <summary> 重み付き抽選のためのピース情報の配列 </summary>
    public WeightedPiece[] WeightedPieces => _weightedPieces;

    [SerializeField] private WeightedPiece[] _weightedPieces;
}
