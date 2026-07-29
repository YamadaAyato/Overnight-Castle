using System;
using UnityEngine;

/// <summary>
///     キャラクターのスキル効果の基底クラス
/// </summary>
[Serializable]
public abstract class CharacterSkillEffectBase
{
    /// <summary>
    ///     スキル効果を実行する
    /// </summary>
    /// <param name="context">スキル効果の実行に必要なコンテキスト情報</param>
    public abstract void ExecuteEffect(CharacterSkillContext context);
}
