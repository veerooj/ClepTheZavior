using SkillTree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class UISkillButton
    {
        private Button button;
        public ScriptableSkill _skill;
        private bool _isUnlocked;
        private bool _isHovered;
        private bool _isSelected;
        
        public static UnityAction<ScriptableSkill> OnSkillButtonClick;

        public UISkillButton(Button assignedButton, ScriptableSkill assignedSkill)
        {
            button = assignedButton;
            _skill = assignedSkill;

            button.clicked += OnClick;
            button.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            button.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            if (assignedSkill != null && assignedSkill.skillIcon != null)
            {
                button.style.backgroundImage = new StyleBackground(assignedSkill.skillIcon);
            }

            ApplyVisuals();
        }

        private void OnClick()
        {
            OnSkillButtonClick?.Invoke(_skill);
        }
        
        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _isHovered = true;
            ApplyVisuals();
        }
        
        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _isHovered = false;
            ApplyVisuals();
        }

        public void VisualState(bool isUnlocked, bool preReqsMet)
        {
            _isUnlocked = isUnlocked;
            ApplyVisuals();
        }
        
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            ApplyVisuals();
        }
        
        private void ApplyVisuals()
        {
            if (_isUnlocked || _isSelected)
            {
                button.style.opacity = 1f; // Si está comprada o seleccionada, alfa a 1
            }
            else
            {
                button.style.opacity = _isHovered ? 0.60f : 0.40f;
            }
            
            Color borderColor = Color.black;

            if (_isHovered)
            {
                borderColor = Color.white; 
            }
            else if (_isUnlocked)
            {
                borderColor = Color.green; 
            }

            button.style.borderTopColor = new StyleColor(borderColor);
            button.style.borderBottomColor = new StyleColor(borderColor);
            button.style.borderLeftColor = new StyleColor(borderColor);
            button.style.borderRightColor = new StyleColor(borderColor);
        }
    }
