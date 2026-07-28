using UnityEngine;

/// <summary>
///     キャラクターの定義を保持するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Overnight Castle/CharacterDefinition")]
public class CharacterDefinition : ScriptableObject
{
    /// <summary> キャラクターの名前 </summary>
    public string CharacterName => _characterName;

    /// <summary> キャラクターのスプライト </summary>
    public Sprite CharacterSprite => _characterSprite;

    /// <summary> 共通ピースに追加するピースセット </summary>
    public PieceSet AdditionalPieceSet => _additionalPieceSet;

    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterSprite;

    [Header("共通ピースに追加するピースセット")]
    [SerializeField] private PieceSet _additionalPieceSet;
}
