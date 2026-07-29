using System;

/// <summary>
///     キャラクターのスキルの実行時情報を保持するクラス
/// </summary>
public class CharacterSkillRuntime
{
    public CharacterSkillRuntime(CharacterSkillDefinition definition)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }

    /// <summary> スキルの定義を保持するプロパティ </summary>
    public CharacterSkillDefinition Definition { get; private set; }

    /// <summary> スキルが使用済みかどうかを示すプロパティ </summary>
    public bool IsUsed { get; private set; }

    /// <summary> スキルが使用可能かどうかを示すプロパティ </summary>
    public bool CanUse => !IsUsed;

    /// <summary>
    ///     スキルを使用する
    /// </summary>
    /// <param name="context">スキルの使用に必要なコンテキスト情報</param>
    /// <returns>スキルの使用に成功した場合はtrue、それ以外の場合はfalse</returns>
    public bool TryUse(CharacterSkillContext context)
    {
        if (!CanUse || context == null)
        {
            return false;
        }
        
        IsUsed = true;
        Definition.Apply(context);
        return true;
    }
}
