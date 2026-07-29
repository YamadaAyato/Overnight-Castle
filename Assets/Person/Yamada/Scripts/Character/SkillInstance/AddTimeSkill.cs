using System;
using UnityEngine;

[Serializable]
public class AddTimeSkill : CharacterSkillEffectBase
{
    public override void ExecuteEffect(CharacterSkillContext context)
    {
        context.AddTime(_addTimeAmount);
    }

    [SerializeField] private float _addTimeAmount;
}
