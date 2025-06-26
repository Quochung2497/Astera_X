using Course.Core;
using Course.UI.Shop;
using UnityEngine.Pool;

namespace Course.ObjectPool
{
    public class ShopSlotUIFactory : BaseFactory<ShopSlotUI>
    {
        public ShopSlotUIFactory(IObjectPool<ShopSlotUI> shopSlotUIPool)
            : base(shopSlotUIPool)
        {
            
        }
    }
}