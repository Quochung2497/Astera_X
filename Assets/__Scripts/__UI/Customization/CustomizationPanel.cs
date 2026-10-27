using System;
using System.Collections.Generic;
using System.Linq;
using Course.Control;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.Achievements;
using Course.Services.Shop;
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

        [Inject] private ICustomization _customization;
        [Inject] private IAchievementManager _achievements;
        [Inject] private IShopService _shop;

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
            if (_shop == null) return;
            _shop.OnPartPurchased   -= HandlePartPurchased;
        }

        private void OnCustomizationLoaded()
        {
            _hasLoaded = true;
            _shop.OnPartPurchased += HandlePartPurchased;
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
            foreach (var tp in bodyToggles.Concat(turretToggles))
            {
                bool byAch  = tp.unlockRequirement == null
                              || _unlocked.Contains(tp.unlockRequirement);

                bool byShop = _shop.PurchasedParts
                    .Any(p => p.partType == tp.partType
                              && p.Index    == tp.index);

                tp.toggle.interactable = byAch || byShop;
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
        
        private void HandlePartPurchased(ShipPartSO part)
        {
            // enable the matching toggle
            var toggles = part.partType == PartType.Body ? bodyToggles : turretToggles;
            foreach (var tp in toggles)
                if (tp.index == part.Index)
                    tp.toggle.interactable = true;
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

