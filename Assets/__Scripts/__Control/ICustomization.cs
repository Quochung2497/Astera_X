using System;
using Course.Core;
using Course.ScriptableObject;
using UnityEngine;

namespace Course.Control
{
    public interface ICustomization
    {
        public int SelectedBodyIndex { get; }
        public int SelectedTurretIndex { get; }
        event Action<PartType, Transform, ShipPartSO.StatModifier[]> OnApplyShipPart;
        event Action OnCustomizationChanged;
        event Action OnLoaded;
        
        void ApplyBody(int index);
        void ApplyTurret(int index);
    }
}