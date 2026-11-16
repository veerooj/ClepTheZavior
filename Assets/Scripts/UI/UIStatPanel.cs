using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStatPanel : MonoBehaviour
{
    private Label skillNameLabel;
    private Label skillDescLabel;
    private Label skillCostLabel;
    private Label dinoCoinsLabel;
    private Label notEnoughTextLabel;
    private Button buyButton;
    
    private UIManager uIManager;

    private void Awake()
    {
        uIManager = GetComponent<UIManager>();
        if (uIManager == null)
        {
            Debug.LogError("UIStatPanel: UIManager not found");
        }
    }

    private void Start()
    {
        uIManager.SkillManager.OnDinoCoinsChanged += PopulateLabelText;
        GatherLabelReferences();
        PopulateLabelText();
    }

    private void GatherLabelReferences()
    {
        skillDescLabel = uIManager.UIDocument.rootVisualElement.Q<Label>("StatDesc");

    }
    
    private void PopulateLabelText()
    {
        skillDescLabel.text = uIManager.Skill.skillDescription;
    }
}
