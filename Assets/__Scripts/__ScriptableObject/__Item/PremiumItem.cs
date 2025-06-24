using UnityEngine;

namespace Course.ScriptableObject.Item
{
    [CreateAssetMenu(menuName = "Shop/Premium IAP Item")]
    // Premium IAP item that unlocks a ship part
    public class PremiumItem : Item
    {
        [Header("Unlockable Part")]
        [Tooltip("ShipPartSO to unlock when purchased")]
        [field: SerializeField] public ShipPartSO PartToUnlock { get; private set; }
    }
}