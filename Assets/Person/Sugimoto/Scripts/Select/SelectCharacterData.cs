using UnityEngine;

[CreateAssetMenu(
    fileName = "SelectCharacterData",
    menuName = "SelectCharacter/Data")]

public class SelectCharacterData : ScriptableObject
{
    public string CharacterName;
    public Sprite CharacterImage;

    //ここにスキルのベースを入れる
    [TextArea]
    public string SkillDescription;
}
