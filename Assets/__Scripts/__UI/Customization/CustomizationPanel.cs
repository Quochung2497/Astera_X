using System;
using System.Collections.Generic;
using System.Linq;
using Course.Control;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.Achievements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility.DependencyInjection;

namespace Course.UI.Customization
{
    public class CustomizationPanel : MonoBehaviour
    {
        [Header("Body Toggles")] [SerializeField]
        private TogglePart[] bodyToggles;

        [Header("Turret Toggles")] [SerializeField]
        private TogglePart[] turretToggles;

        [Inject] private CustomizationManager _customization;
        [Inject] private IAchievementManager _achievements;
        
        private bool _hasLoaded = false;
        private readonly HashSet<AchievementSO> _unlocked = new();

        private void Awake()
        {
            // wire up the click callbacks once
            foreach (var tp in bodyToggles)
                tp.toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn && tp.toggle.interactable)
                        _customization.ApplyBody(tp.index);
                });

            foreach (var tp in turretToggles)
                tp.toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn && tp.toggle.interactable)
                        _customization.ApplyTurret(tp.index);
                });
            
            if (_customization == null || _achievements == null)
                return;
            _achievements.OnUnlocked += OnAchievementUnlocked;
            _customization.OnLoaded += OnCustomizationLoaded;
        }

        private void OnDestroy()
        {
            if (_customization == null || _achievements == null)
                return;
            _achievements.OnUnlocked -= OnAchievementUnlocked;
            _customization.OnLoaded -= OnCustomizationLoaded;
        }

        private void OnCustomizationLoaded()
        {
            // the manager has already set SelectedBodyIndex / SelectedTurretIndex
            // now make sure the actual ship is built to that state:
            _customization.ApplyBody(_customization.SelectedBodyIndex);
            _customization.ApplyTurret(_customization.SelectedTurretIndex);
            _hasLoaded = true;

            // and then refresh the toggles
            InitializeUI();
        }

        private void OnAchievementUnlocked(AchievementSO def)
        {
            _unlocked.Add(def);
            if (!_hasLoaded) return;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // 1) Enable/disable each toggle
            foreach (var tp in bodyToggles)
            {
                bool unlocked = tp.unlockRequirement == null || _unlocked.Contains(tp.unlockRequirement);
                tp.toggle.interactable = unlocked;
            }

            foreach (var tp in turretToggles)
            {
                bool unlocked = tp.unlockRequirement == null || _unlocked.Contains(tp.unlockRequirement);
                tp.toggle.interactable = unlocked;
            }

            // 2) Set each toggle’s isOn to match the current selection
            foreach (var tp in bodyToggles)
            {
                if (tp.toggle.interactable)
                    tp.toggle.SetIsOnWithoutNotify(tp.index == _customization.SelectedBodyIndex);
                else
                    tp.toggle.SetIsOnWithoutNotify(false);
            }

            foreach (var tp in turretToggles)
            {
                if (tp.toggle.interactable)
                    tp.toggle.SetIsOnWithoutNotify(tp.index == _customization.SelectedTurretIndex);
                else
                    tp.toggle.SetIsOnWithoutNotify(false);
            }
        }
    }
    [Serializable]
    public struct TogglePart
    {
        public PartType        partType;            
        public int             index;               // which slot
        public AchievementSO   unlockRequirement;   // null = always unlocked
        public Toggle          toggle;              
    }
}

