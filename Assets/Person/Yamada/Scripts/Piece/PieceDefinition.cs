using UnityEngine;

/// <summary>
///     城を構成するパーツの定義を表すScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "PieceDefinition", menuName = "Overnight Castle/PieceDefinition")]
public class PieceDefinition : ScriptableObject
{
    /// <summary> このパーツのスプライト </summary>
    public Sprite Sprite => _sprite;

    /// <summary> このパーツの種類 </summary>
    public PieceType PieceType => _pieceType;

    /// <summary> このパーツの城の部位の種類 </summary>
    public CastlePartType PartType => _partType;

    /// <summary> このパーツのスコア </summary>
    public int Score => _score;

    /// <summary> このパーツの物理設定 </summary>
    public PiecePhysicsSettings PhysicsSettings => _physicsSettings;

    /// <summary> このパーツのPrefab </summary>
    public FallingPiece PrefabOverride => _prefab;

    [Header("表示設定")]
    [SerializeField] private Sprite _sprite;

    [Header("パーツ設定")]
    [SerializeField] private PieceType _pieceType = PieceType.Normal;
    [SerializeField] private CastlePartType _partType = CastlePartType.Foundation;

    [Header("スコア設定")]
    [SerializeField] private int _score = 0;

    [Header("物理設定")]
    [SerializeField] private PiecePhysicsSettings _physicsSettings = new();

    [Header("Prefab設定、特殊処理が必要な場合のみ")]
    [SerializeField, Tooltip("このパーツのPrefabを指定します。")] private FallingPiece _prefab;
}
