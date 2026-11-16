using System;
using System.Collections.Generic;
using SkillTree;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ScriptableObjectLibrary skillLibrary;
    [SerializeField] ScriptableSkill selectedSkill;
    
    [SerializeField] private VisualTreeAsset uiSkillButton;
    //private List<UISkillButton> skillButtons;
    
    private SkillManager skillManager;
    public SkillManager SkillManager => skillManager;

    public ScriptableSkill Skill => selectedSkill;
    
    private UIDocument uiDocument;
    public UIDocument UIDocument => uiDocument;

    private List<UISkillButton> _skillButtons = new List<UISkillButton>();

    public Action OnSkillSelected;
    
    
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        skillManager = FindFirstObjectByType<SkillManager>();
    }
    
    private void OnEnable()
    {
        UISkillButton.OnSkillButtonClick += SelectSkill;
        if (skillManager != null)
        {
            skillManager.OnDinoCoinsChanged += RefreshAllButtons;
        }
    }
    
    private void OnDisable()
    {
        UISkillButton.OnSkillButtonClick -= SelectSkill;
        if (skillManager != null)
        {
            skillManager.OnDinoCoinsChanged -= RefreshAllButtons;
        }
    }
    
    private void Start()
    {
        BuildSkillTreeUI();
        RefreshAllButtons();
    }

    private void BuildSkillTreeUI()
    {
        if (skillLibrary == null || uiDocument == null)
        {
            Debug.LogError("UIManager: Falta asignar SkillLibrary o UIDocument.");
            return;
        }
        
        
        VisualElement root = uiDocument.rootVisualElement;
        _skillButtons.Clear();
        
        //DINO BRANCH
        PopulateBranch(root, "DinoParty", skillLibrary.GetSkillsByBranch(SkillBranch.DinoParty));
        PopulateBranch(root, "DinCoin", skillLibrary.GetSkillsByBranch(SkillBranch.DinCoin));
        PopulateBranch(root, "24kDino", skillLibrary.GetSkillsByBranch(SkillBranch.Dino24k));
        
        //PLAYER BRANCH
        PopulateBranch(root, "ZoubleJump", skillLibrary.GetSkillsByBranch(SkillBranch.ZoubleJump));
        PopulateBranch(root, "ZetPack", skillLibrary.GetSkillsByBranch(SkillBranch.ZetPack));
        PopulateBranch(root, "ClepRescue", skillLibrary.GetSkillsByBranch(SkillBranch.ClepRescue));
        
        //GUN BRANCH
        List<ScriptableSkill> gunSkills = skillLibrary.GetSkillsByBranch(SkillBranch.Capazity);
        PopulateBranch(root, "Capazity_1_4", gunSkills.FindAll(s => s.skillTier >= 1 && s.skillTier <= 4));
        PopulateBranch(root, "Capazity_5_8", gunSkills.FindAll(s => s.skillTier >= 5 && s.skillTier <= 8));
        
        //STRUCTURE BRANCH
        PopulateBranch(root, "ZaveThem", skillLibrary.GetSkillsByBranch(SkillBranch.ZaveThem));
    }

    private void PopulateBranch(VisualElement root, string containerName, List<ScriptableSkill> skills)
    {
        VisualElement container = root.Q<VisualElement>(containerName);
        if (container == null)
        {
            Debug.LogWarning($"No se encontró el contenedor '{containerName}' en el UXML.");
            return;
        }
        container.Clear();

        foreach (ScriptableSkill skill in skills)
        {
            VisualElement buttonInstance = uiSkillButton.Instantiate();
            Button b = buttonInstance.Q<Button>("SkillBtn");

            if (b == null)
            {
                Debug.LogError("No se encontró un Button llamado 'SkillBtn' dentro de SkillButton.uxml");
                return;
            }

            b.text = $"T{skill.skillTier}";

            UISkillButton skillBtn = new UISkillButton(b, skill);
            _skillButtons.Add(skillBtn);

            container.Add(buttonInstance);
        }

    }
    
    public void SelectSkill(ScriptableSkill newSkill)
    {
        selectedSkill = newSkill;
        foreach (UISkillButton btn in _skillButtons)
        {
            btn.SetSelected(btn._skill == selectedSkill);
        }
        OnSkillSelected?.Invoke();
    }

    public void RefreshAllButtons()
    {
        if (skillManager == null) return;

        foreach (UISkillButton btn in _skillButtons)
        {
            bool unlocked = skillManager.IsSkillUnlocked(btn._skill);
            bool preReqs = skillManager.PreReqsMet(btn._skill);
            btn.VisualState(unlocked, preReqs);
        }
    }
}
