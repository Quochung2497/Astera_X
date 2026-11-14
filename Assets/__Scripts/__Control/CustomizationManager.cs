using System;
using System.Linq;
using System.Threading.Tasks;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.SaveService;
using UnityEngine;

namespace Course.Control
{
    [DisallowMultipleComponent]
    public class CustomizationManager : MonoBehaviour, ISavable, ICustomization
    {
        [Header("All available parts")]
        public ShipPartSO[] bodyParts;    // indexes 0..N
        public ShipPartSO[] turretParts;  // indexes 0..N

        [Header("Where to mount the spawned prefabs")]
        [SerializeField] private Transform bodyMount;
        [SerializeField] private Transform turretMount;

        private ShipPartSelection<ShipPartSO> _selection;
        private GameObject _currentBody;
        private GameObject _currentTurret;
        public int SelectedBodyIndex => _selection.SelectedBodyIndex;
        public int SelectedTurretIndex => _selection.SelectedTurretIndex;
        
        
        private void Awake()
        {
            var catalog = bodyParts.Where(part => part.partType == PartType.Body)
                .Concat(turretParts.Where(part => part.partType == PartType.Turret))
                .ToDictionary(part => (part.partType, part.Index));
            _selection = new ShipPartSelection<ShipPartSO>(catalog);
        }

        public event Action<PartType, Transform, ShipPartSO.StatModifier[]> OnApplyShipPart;
        public event Action OnCustomizationChanged;
        public event Action OnLoaded;


        public void ApplyBody(int index) => ApplyPart(PartType.Body, index, ref _currentBody, bodyMount);

        public void ApplyTurret(int index) => ApplyPart(PartType.Turret, index, ref _currentTurret, turretMount);

        private void ApplyPart(PartType type, int index, ref GameObject current, Transform mount)
        {
            if (!_selection.TryGetPart(type, index, out var part)) return;

            SwapPrefab(ref current, part.GetPrefab(), mount, rotateAsTurret: type == PartType.Turret);
            _selection.SetSelectedIndex(type, index);
            OnApplyShipPart?.Invoke(type, current != null ? current.transform : mount, part.modifiers);
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
            
            _selection.RestoreIndices(data.selectedBody, data.selectedTurret);
            ApplyBody(data.selectedBody);
            ApplyTurret(data.selectedTurret);
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

