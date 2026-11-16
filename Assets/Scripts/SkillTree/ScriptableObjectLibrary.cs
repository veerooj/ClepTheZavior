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
        return SkillLibrary.Where(skill => skill.skillTier == tier).ToList();
    }
    
    public List<ScriptableSkill> GetSkillsBySector(SkillSector sector)
    {
        return SkillLibrary
            .Where(skill => skill.sector == sector)
            .OrderBy(skill => skill.skillTier)
            .ToList();
    }

    public List<ScriptableSkill> GetSkillsByBranch(SkillBranch branch)
    {
        return SkillLibrary
            .Where(skill => skill.branch == branch)
            .OrderBy(skill => skill.skillTier)
            .ToList();
    }
    public ScriptableSkill GetSkill(SkillBranch branch, int tier)
    {
        return SkillLibrary.FirstOrDefault(skill => skill.branch == branch && skill.skillTier == tier);
    }
    
}
