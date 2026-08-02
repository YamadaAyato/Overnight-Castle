using System;
using UnityEngine;

/// <summary>
///     キャラクターのスキルを管理するコントローラークラス
/// </summary>
public class CharacterSkillController : MonoBehaviour
{
    /// <summary> 1つ目のスキルの定義 </summary>
    public CharacterSkillDefinition FirstSkillDefinition => _firstSkillRuntime?.Definition;

    /// <summary> 2つ目のスキルの定義 </summary>
    public CharacterSkillDefinition SecondSkillDefinition => _secondSkillRuntime?.Definition;

    /// <summary> 1つ目のスキルが使用済みかどうかを示すプロパティ </summary>
    public bool IsFirstSkillUsed => _firstSkillRuntime?.IsUsed ?? false;

    /// <summary> 2つ目のスキルが使用済みかどうかを示すプロパティ </summary>
    public bool IsSecondSkillUsed => _secondSkillRuntime?.IsUsed ?? false;

    /// <summary> スキルが使用されたときに発火するイベント </summary>
    public event Action<CharacterSkillSlot, CharacterSkillDefinition> OnSkillUsed;

    /// <summary>
    ///     CharacterSkillControllerを初期化します。
    /// </summary>
    /// <param name="characterDefinition">キャラクターの定義</param>
    /// <param name="pieceSpawnModifiers">ピース生成の修飾子</param>
    /// <param name="timer">タイマー</param>
    /// <param name="inGameManager">InGameManagerのインスタンス</param>
    /// <returns>初期化が成功したかどうか</returns>
    public bool Initialize(
        CharacterDefinition characterDefinition,
        PieceSpawnModifiers pieceSpawnModifiers,
        Timer timer,
        InGameManager inGameManager)
    {
        if (characterDefinition == null ||
            characterDefinition.Skill1 == null ||
            characterDefinition.Skill2 == null)
        {
            Debug.LogError("CharacterDefinitionがnullです", this);
            return false;
        }

        if (pieceSpawnModifiers == null)
        {
            Debug.LogError("PieceSpawnModifiersがnullです", this);
            return false;
        }

        if (timer == null)
        {
            Debug.LogError("Timerがnullです", this);
            return false;
        }

        if (inGameManager == null)
        {
            Debug.LogError("InGameManagerがnullです", this);
            return false;
        }

        _skillContext =
            new CharacterSkillContext(pieceSpawnModifiers, timer, inGameManager, destroyCancellationToken);
        _firstSkillRuntime =
            new CharacterSkillRuntime(characterDefinition.Skill1);
        _secondSkillRuntime =
            new CharacterSkillRuntime(characterDefinition.Skill2);
        _canUseSkills = true;

        return true;
    }

    /// <summary>
    ///     1つ目のスキルを使用します。
    /// </summary>
    public void UseFirstSkill()
    {
        TryUseSkill(CharacterSkillSlot.Skill1);
    }

    /// <summary>
    ///     2つ目のスキルを使用します。
    /// </summary>
    public void UseSecondSkill()
    {
        TryUseSkill(CharacterSkillSlot.Skill2);
    }

    /// <summary>
    ///     指定されたスキルスロットのスキルを使用しようとします。
    /// </summary>
    /// <param name="skillSlot">使用するスキルのスロット</param>
    /// <returns>スキルの使用が成功したかどうか</returns>
    public bool TryUseSkill(CharacterSkillSlot skillSlot)
    {
        if (!_canUseSkills ||
            _skillContext == null)
        {
            return false;
        }

        CharacterSkillRuntime skillRuntime = skillSlot switch
        {
            CharacterSkillSlot.Skill1 => _firstSkillRuntime,
            CharacterSkillSlot.Skill2 => _secondSkillRuntime,
            _ => null
        };

        if (skillRuntime == null ||
            !skillRuntime.TryUse(_skillContext))
        {
            return false;
        }

        OnSkillUsed?.Invoke(skillSlot, skillRuntime.Definition);

        return true;
    }

    /// <summary>
    ///     全てのスキルの使用を停止する。
    /// </summary>
    public void StopAllSkills()
    {
        _canUseSkills = false;
    }

    private CharacterSkillContext _skillContext;
    private CharacterSkillRuntime _firstSkillRuntime;
    private CharacterSkillRuntime _secondSkillRuntime;
    private bool _canUseSkills;
}
