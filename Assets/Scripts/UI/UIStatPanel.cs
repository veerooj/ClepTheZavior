using System;
using SkillTree;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStatPanel : MonoBehaviour
{
    private UIManager uIManager;
    private ScriptableSkill skill;
    
    private Label skillTitleabel, skillDescLabel,skillCostLabel,dinoCoinsLabel,notEnoughTextLabel;
    private Button buyButton;

    private void Awake()
    {
        uIManager = GetComponent<UIManager>();
        if (uIManager == null)
        {
            Debug.LogError("UIStatPanel: UIManager not found");
        }
    }

    private void OnEnable()
    {
        if (uIManager != null)
        {
            uIManager.OnSkillSelected += UpdateUI;
            if (uIManager.SkillManager != null)
            {
                uIManager.SkillManager.OnDinoCoinsChanged += UpdateUI;
            }
        }
    }

    private void OnDisable()
    {
        if (uIManager != null)
        {
            uIManager.OnSkillSelected -= UpdateUI;
            if (uIManager.SkillManager != null)
            {
                uIManager.SkillManager.OnDinoCoinsChanged -= UpdateUI;
            }
        }
    }

    private void Start()
    {
        GatherLabelReferences();
        RegisterCallbacks();
        UpdateUI();
    }

    private void GatherLabelReferences()
    {
        if (uIManager == null || uIManager.UIDocument == null || uIManager.UIDocument.rootVisualElement == null) return;

        VisualElement root = uIManager.UIDocument.rootVisualElement;
        
        skillTitleabel = root.Q<Label>("SkillName");
        skillDescLabel = root.Q<Label>("SkillDesc");
        skillCostLabel = root.Q<Label>("SkillCost");
        dinoCoinsLabel = root.Q<Label>("DinoCoins");
        notEnoughTextLabel = root.Q<Label>("NotEnoughText");
        buyButton = root.Q<Button>("BuyButton");
        
        if (buyButton != null)
        {
            buyButton.style.opacity = 1f;
            buyButton.SetEnabled(true);
        }
    }
    
    private void RegisterCallbacks()
    {
        if (buyButton != null)
        {
            buyButton.clicked += OnBuyButtonClicked;
        }
    }
    
    private void OnBuyButtonClicked()
    {
        var skill = uIManager.Skill;
        if (skill == null || uIManager.SkillManager == null) return;
        
        if (uIManager.SkillManager.IsSkillUnlocked(skill)) return;
        
        if (!uIManager.SkillManager.PreReqsMet(skill))
        {
            notEnoughTextLabel.text = "Prerequisites not met";
            notEnoughTextLabel.style.display = DisplayStyle.Flex;
            return;
        }
        
        if (!uIManager.SkillManager.canAffordSkill(skill))
        {
            notEnoughTextLabel.text = "Not enough coins";
            notEnoughTextLabel.style.display = DisplayStyle.Flex;
            return;
        }
        
        uIManager.SkillManager.UnlockSkill(skill);
        uIManager.RefreshAllButtons();
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (dinoCoinsLabel == null) return;

        if (uIManager.SkillManager != null)
        {
            dinoCoinsLabel.text = $"Dino Coins: {uIManager.SkillManager.dinoCoins}";
        }

        var skill = uIManager.Skill;
        
        notEnoughTextLabel.style.display = DisplayStyle.None;
        buyButton.style.opacity = 1f;
        buyButton.SetEnabled(true);

        if (skill == null)
        {
            skillTitleabel.text = "Select a Skill";
            skillDescLabel.text = "";
            skillCostLabel.text = "";
            buyButton.SetEnabled(false);
            notEnoughTextLabel.style.display = DisplayStyle.None;
            return;
        }

        skillTitleabel.text = skill.skillName;
        skillDescLabel.text = skill.skillDescription;
        skillCostLabel.text = $"Cost: {skill.cost}";

        bool isUnlocked = uIManager.SkillManager.IsSkillUnlocked(skill);
        bool canAfford = uIManager.SkillManager.canAffordSkill(skill);
        bool preReqsMet = uIManager.SkillManager.PreReqsMet(skill);

        if (isUnlocked)
        {
            buyButton.text = "UNLOCKED";
            buyButton.SetEnabled(false);
            notEnoughTextLabel.style.display = DisplayStyle.None;
        }
        else if (!canAfford)
        {
            buyButton.text = "BUY SKILL";
            buyButton.SetEnabled(false);
            notEnoughTextLabel.text = "Not enough coins";
            notEnoughTextLabel.style.display = DisplayStyle.Flex;
        }
        else if (!preReqsMet)
        {
            buyButton.text = "LOCKED";
            buyButton.SetEnabled(false);
            notEnoughTextLabel.text = "Prerequisites not met";
            notEnoughTextLabel.style.display = DisplayStyle.Flex;
        }
        else
        {
            buyButton.text = "BUY SKILL";
            buyButton.SetEnabled(true);
            notEnoughTextLabel.style.display = DisplayStyle.None;
        }
    }
    
}
