using System.Collections.Generic;
using System.Linq;
using SkillTree;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Library", menuName = "Skill System/New Skill Library")]
public class ScriptableObjectLibrary : ScriptableObject
{
    public List<ScriptableSkill> SkillLibrary;

    public List<ScriptableSkill> GetSkillsOfTier(int tier)
    {
        return SkillLibrary.Where(skill  => skill.skillTier == tier).ToList();
    }

}
