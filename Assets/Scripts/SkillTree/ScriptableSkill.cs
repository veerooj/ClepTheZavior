using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SkillTree
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
    public class ScriptableSkill : ScriptableObject
    {
        public List<UpgradeData> UpgradeData =  new List<UpgradeData>();
        public bool IsAbility;
        public string skillName;

        public bool overwriteDescription;
        [TextArea(1,4)] public string skillDescription;
        public Sprite skillIcon;
        public List<ScriptableSkill> SkillPrerequisites =  new List<ScriptableSkill>();
        public int skillTier;
        public int cost;
        
        private void OnValidate()
        {
            if (skillName == "") skillName = name;
            if (UpgradeData.Count == 0) return;
            if (overwriteDescription) return;
       
            GenerateDescription();

        }

        private void GenerateDescription()
        {
            if (IsAbility)
            {
                switch (UpgradeData[0].StatType)
                {
                    case StateTypes.playerDoubleJump:
                        skillDescription = $"{skillName} grants Clep the ability to jump 2 times on the air";
                        break;
                    case StateTypes.playerJetpack:
                        skillDescription = $"{skillName} grants Clep a Jetpack which will make them float in the air";
                        break;
                   
                }
            }
            
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{skillName} increases ");
                for (int i =0; i < UpgradeData.Count; i++)
                {
                    sb.Append(UpgradeData[i].StatType.ToString());
                    sb.Append(" by ");
                    sb.Append(UpgradeData[i].skillIncreaseAmount.ToString());
                    sb.Append(UpgradeData[i].isPercentage ? "%" : "point(s)");
                    if (i == UpgradeData.Count - 2) sb.Append(" and ");
                    sb.Append(i  < UpgradeData.Count - 1 ? ", " : ".");
                }
                
                skillDescription = sb.ToString();
                
            }
        }
    }
    
    [System.Serializable]
    public class UpgradeData
    {
        public StateTypes StatType;
        public int skillIncreaseAmount;
        public bool isPercentage;
    }

    public enum StateTypes
    {
        dinoSpawnTime,
        dinoValue,
        dinoGoldenChance,
        playerDoubleJump,
        playerJetpack,
        playerDeathLoot,
        playerJetpackCapacity,
        gunCapacity,
        domeParts
    }
}
