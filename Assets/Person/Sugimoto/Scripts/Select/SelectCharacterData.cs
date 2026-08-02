using UnityEngine;

/// <summary>
///     キャラクター選択画面で使用するキャラクターデータを保持するSO
/// </summary>
[CreateAssetMenu(
    fileName = "SelectCharacterData",
    menuName = "SelectCharacter/Data")]

public class SelectCharacterData : ScriptableObject
{
    /// <summary> キャラクターの定義 </summary>
    public CharacterDefinition CharacterDefinition => _characterDefinition;

    /// <summary> キャラクターの名前 </summary>
    public string CharacterName => _characterDefinition.CharacterName;

    /// <summary> キャラクターのスプライト </summary>
    public Sprite CharacterSprite => _characterDefinition.CharacterSprite;

    /// <summary> キャラクター選択時のスプライト </summary>
    public Sprite SelectedCharacterSprite => _characterDefinition.SelectedCharacterSprite;

    /// <summary> スキル1の名前 </summary>
    public string SkillName1 => _characterDefinition.Skill1.SkillName;

    /// <summary> スキル1の説明 </summary>
    public string SkillDescription1 => _characterDefinition.Skill1.SkillDescription;

    /// <summary> スキル2の名前 </summary>
    public string SkillName2 => _characterDefinition.Skill2.SkillName;

    /// <summary> スキル2の説明 </summary>
    public string SkillDescription2 => _characterDefinition.Skill2.SkillDescription;

    [SerializeField] private CharacterDefinition _characterDefinition;
}
