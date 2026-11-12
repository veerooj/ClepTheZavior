using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SkillTree
{
    public class SkillManager : MonoBehaviour
    {
        [Header("Dino Values)")] private float _dinoSpawnTime, _dinoValue, _dinoGoldenChance;

        [Header("Player Values")] [Header("Abilities")]
        private int _playerDoubleJump, _playerJetpack;

        private float _playerDeathLoot, _playerJetpackCapacity;

        [Header("Gun Values")] private float _gunCapacity;

        [Header("Dome Values")] private int _domeParts;

        private int _dinoCoins;

        public float dinoSpawnTime => _dinoSpawnTime;
        public float dinoValue  => _dinoValue;
        public float dinoGoldenChance  => _dinoGoldenChance;
        public float playerDeathLoot  => _playerDeathLoot;
        public float playerJetpackCapacity  => _playerJetpackCapacity;
        public float gunCapacity => _gunCapacity;
        public int domeParts => _domeParts;
    
        public bool isDoubleJump => _playerDoubleJump > 0;
        public bool isJetpackCapacity => _playerJetpackCapacity > 0;
    
        public int dinoCoins => _dinoCoins;

        public UnityAction OnDinoCoinsChanged;
        
        private List<ScriptableSkill> _unlockedSkills = new List<ScriptableSkill>();

        private void Awake()
        {
            _dinoCoins = 10;
        }

        public void GainDinoCoin() //this is placeholder until we get the dinos thingy 
        {
            _dinoCoins++;
            OnDinoCoinsChanged?.Invoke();
        }

        public bool canAffordSkill(ScriptableSkill skill)
        {
            return _dinoCoins >= skill.cost;
        }

        public void UnlockSkill(ScriptableSkill skill)
        {
            if (!canAffordSkill(skill)) return;
            ModifyStats(skill);
            _unlockedSkills.Add(skill);
            _dinoCoins -= skill.cost;
            OnDinoCoinsChanged?.Invoke();
            
        }

        private void ModifyStats(ScriptableSkill skill)
        {
            foreach (UpgradeData data in skill.UpgradeData)
            {
                switch (data.StatType)
                {
                    case StateTypes.dinoGoldenChance:
                        ModifyStat(ref _dinoGoldenChance, data);
                        break;
                    case StateTypes.dinoSpawnTime:
                        ModifyStat(ref _dinoSpawnTime, data);
                        break;
                    case StateTypes.dinoValue:
                        ModifyStat(ref _dinoValue, data);
                        break;
                    case StateTypes.playerDeathLoot:
                        ModifyStat(ref _playerDeathLoot, data);
                        break;
                    case StateTypes.playerJetpackCapacity:
                        ModifyStat(ref _playerJetpackCapacity, data);
                        break;
                    case StateTypes.playerJetpack:
                        ModifySkill(ref _playerJetpack, data);
                        break;
                    case StateTypes.gunCapacity:
                        ModifyStat(ref _gunCapacity, data);
                        break;
                    case StateTypes.domeParts:
                        ModifySkill(ref _domeParts, data);
                        break;
                    case StateTypes.playerDoubleJump:
                        ModifySkill(ref _playerDoubleJump, data);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool IsSkillUnlocked(ScriptableSkill skill)
        {
            return _unlockedSkills.Contains(skill);
        }

        public bool PreReqsMet(ScriptableSkill skill)
        {
            return skill.SkillPrerequisites.Count==0 || skill.SkillPrerequisites.All(_unlockedSkills.Contains);
        }

        private void ModifyStat(ref float stat, UpgradeData data)
        {
            if (data.isPercentage) stat += (int)(stat * (data.skillIncreaseAmount / 100f));
            else stat += data.skillIncreaseAmount;
        }
        private void ModifySkill(ref int stat, UpgradeData data)
        {
                stat += data.skillIncreaseAmount;
        }
        
    
    }
}

