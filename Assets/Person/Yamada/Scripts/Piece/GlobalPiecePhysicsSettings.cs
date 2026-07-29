using UnityEngine;

/// <summary>
///     すべてのピースへ適用させる場合の物理設定を保持するSO
/// </summary>
[CreateAssetMenu(fileName = "GlobalPiecePhysicsSettings", menuName = "Overnight Castle/GlobalPiecePhysicsSettings")]
public class GlobalPiecePhysicsSettings : ScriptableObject
{
    /// <summary> ピースの物理設定 </summary>
    public PiecePhysicsSettings PiecePhysicsSettings => _piecePhysicsSettings;

    [SerializeField] private PiecePhysicsSettings _piecePhysicsSettings = new();
}
