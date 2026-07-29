using System.Collections.Generic;
using Template.Editor;
using UnityEngine;

/// <summary>
///     キャラクターのスキル定義を表すScriptableObject
/// </summary>
[CreateAssetMenu(
    fileName = "CharacterSkillDefinition",
    menuName = "Overnight Castle/CharacterSkillDefinition")]
public class CharacterSkillDefinition : ScriptableObject
{
    /// <summary> スキル名 </summary>
    public string SkillName => _skillName;

    /// <summary> スキルの説明 </summary>
    public string SkillDescription => _skillDescription;

    /// <summary> スキルの効果一覧 </summary>
    public IReadOnlyList<CharacterSkillEffectBase> SkillEffects => _skillEffects;


    [Header("スキル情報")]
    [SerializeField] private string _skillName;
    [SerializeField, TextArea] private string _skillDescription;

    [Header("スキルの効果")]
    [SerializeReference, SubclassSelector]
    private List<CharacterSkillEffectBase> _skillEffects = new();

    /// <summary>
    ///     スキルを発動する
    /// </summary>
    /// <param name="context">スキルの発動に必要なコンテキスト情報</param>
    public void Apply(CharacterSkillContext context)
    {
        if (context == null)
        {
            Debug.LogError("CharacterSkillContextがnullです。");
            return;
        }

        foreach (var effect in _skillEffects)
        {
            if (effect != null)
            {
                effect.ExecuteEffect(context);
            }
        }
    }
}
