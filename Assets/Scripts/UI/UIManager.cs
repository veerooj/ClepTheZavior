using System;
using SkillTree;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private SkillManager skillManager;
    public SkillManager SkillManager => skillManager;
    
    [SerializeField] ScriptableSkill selectedSkill;
    public ScriptableSkill Skill => selectedSkill;
    
    private UIDocument uiDocument;
    public UIDocument UIDocument => uiDocument;
    
    public Action OnSkillSelected;
    
    
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        skillManager = FindFirstObjectByType<SkillManager>();
    }
    
    public void SelectSkill(ScriptableSkill newSkill)
    {
        selectedSkill = newSkill;
        OnSkillSelected?.Invoke();
    }
}
