using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.Attribute;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.Achievements;
using Course.Services.SaveService;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Control
{
    [DisallowMultipleComponent]
    public class CustomizationManager : MonoBehaviour, ISavable , ICustomization
    {
        [Header("All available parts")]
        public ShipPartSO[] bodyParts;    // indexes 0..N
        public ShipPartSO[] turretParts;  // indexes 0..N

        [Header("Where to mount the spawned prefabs")]
        [SerializeField] private Transform bodyMount;
        [SerializeField] private Transform turretMount;

        private Dictionary<int, ShipPartSO> _bodyByIndex;
        private Dictionary<int, ShipPartSO> _turretByIndex;
        private GameObject _currentBody;
        private GameObject _currentTurret;
        public int SelectedBodyIndex  { get; private set; }
        public int SelectedTurretIndex{ get; private set; }
        
        
        private void Awake()
        {
            // build fast index → SO maps
            _bodyByIndex = bodyParts
                .Where(so => so.partType == PartType.Body)
                .ToDictionary(so => so.Index, so => so);

            _turretByIndex = turretParts
                .Where(so => so.partType == PartType.Turret)
                .ToDictionary(so => so.Index, so => so);
        }

        public event Action<PartType, Transform, ShipPartSO.StatModifier[]> OnApplyShipPart;
        public event Action OnCustomizationChanged;
        public event Action OnLoaded;


        public void ApplyBody(int index)
        {
            if (!_bodyByIndex.TryGetValue(index, out var so)) return;
            SwapPrefab(ref _currentBody, so.GetPrefab(), bodyMount);
            SelectedBodyIndex = index;
            Transform newBodyTransform = _currentBody != null
                ? _currentBody.transform
                : bodyMount;
            OnApplyShipPart?.Invoke(PartType.Body, newBodyTransform, so.modifiers);
            OnCustomizationChanged?.Invoke();
        }

        public void ApplyTurret(int index)
        {
            if (!_turretByIndex.TryGetValue(index, out var so)) return;
            
            SwapPrefab(ref _currentTurret, so.GetPrefab(), turretMount, rotateAsTurret: true);
            SelectedTurretIndex = index;
            Transform newTurretTransform = _currentTurret != null
                ? _currentTurret.transform
                : turretMount;
            OnApplyShipPart?.Invoke(PartType.Turret,newTurretTransform, so.modifiers);
            OnCustomizationChanged?.Invoke();
        }

        private void SwapPrefab(ref GameObject existing, GameObject prefab, Transform mount, bool rotateAsTurret = false)
        {
            if (existing != null)
                Destroy(existing);

            if (prefab != null)
            {
                existing = Instantiate(prefab, mount);
                existing.transform.localPosition = Vector3.zero;
                existing.transform.localRotation = rotateAsTurret
                    ? Quaternion.Euler(90f, 0f, 0f)
                    : Quaternion.identity;
            }
        }
        
        object ISavable.GetDefault() => new ShipCustomizationData();

        async Task ISavable.Load(ISaveService svc)
        {
            // Load or default
            var defaultData = (ShipCustomizationData)(this as ISavable).GetDefault();
            var data = await svc.LoadData("Customization", defaultData, this);
            
            // Apply loaded indices
            SelectedBodyIndex   = data.selectedBody;
            SelectedTurretIndex = data.selectedTurret;
            ApplyBody(SelectedBodyIndex);
            ApplyTurret(SelectedTurretIndex);
            OnLoaded?.Invoke();
        }

        async Task ISavable.Save(ISaveService svc)
        {
            var data = new ShipCustomizationData
            {
                selectedBody   = SelectedBodyIndex,
                selectedTurret = SelectedTurretIndex
            };
            await svc.SaveData("Customization", data, this);
            // Debug.Log($"[CustomizationManager] Saved body={data.selectedBody}, turret={data.selectedTurret}");
        }

    } 
    
    [Serializable]
    public class ShipCustomizationData
    {
        public int selectedBody   = 0;
        public int selectedTurret = 0;
    }
}

