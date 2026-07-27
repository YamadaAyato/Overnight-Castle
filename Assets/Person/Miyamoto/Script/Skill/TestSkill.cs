using UnityEngine;

/// <summary>
///     手本となるスキルの書き方
/// </summary>
[System.Serializable]
public class TestSkill : ISkill
{
    [SerializeField] private StageSettings _stageSettings;
    public void SkillEffect()
    {
        throw new System.NotImplementedException();
    }
}
