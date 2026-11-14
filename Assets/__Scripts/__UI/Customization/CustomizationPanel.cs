using System;
using Course.Control;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.Achievements;
using Course.Services.Shop;
using UnityEngine;
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
        private readonly ShipPartAvailability _availability = new();

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
            _shop.OnShopLoaded += OnShopLoaded;

        }

        private void OnDestroy()
        {
            if (_customization == null || _achievements == null)
                return;
            _achievements.OnUnlocked -= OnAchievementUnlocked;
            _customization.OnLoaded -= OnCustomizationLoaded;
            _shop.OnShopLoaded -= OnShopLoaded;
            if (_hasLoaded)
                _shop.OnPartPurchased -= HandlePartPurchased;
        }

        private void OnCustomizationLoaded()
        {
            if (!_hasLoaded)
                _shop.OnPartPurchased += HandlePartPurchased;

            _hasLoaded = true;
            OnShopLoaded();
        }

        private void OnAchievementUnlocked(AchievementSO def)
        {
            _availability.UnlockAchievement(def.GetInstanceID());
            if (_hasLoaded)
                RefreshToggles();
        }

        private void OnShopLoaded()
        {
            foreach (var part in _shop.PurchasedParts)
                _availability.RecordPurchase(part.partType, part.Index);

            if (_hasLoaded)
                RefreshToggles();
        }

        private void RefreshToggles()
        {
            RefreshToggles(bodyToggles, _customization.SelectedBodyIndex);
            RefreshToggles(turretToggles, _customization.SelectedTurretIndex);
        }

        private void RefreshToggles(TogglePart[] toggles, int selectedIndex)
        {
            foreach (var part in toggles)
            {
                int? requirement = part.unlockRequirement == null ? null : part.unlockRequirement.GetInstanceID();
                bool canSelect = _availability.CanSelect(part.partType, part.index, requirement);
                part.toggle.interactable = canSelect;
                part.toggle.SetIsOnWithoutNotify(canSelect && part.index == selectedIndex);
            }
        }


        private void HandlePartPurchased(ShipPartSO part)
        {
            _availability.RecordPurchase(part.partType, part.Index);
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

