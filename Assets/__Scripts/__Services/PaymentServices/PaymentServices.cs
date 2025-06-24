using Course.ScriptableObject.Item;
using UnityEngine;

namespace Course.Services
{
    //TODO: Temporary class to test Payment functionality, will be replaced with a proper implementation later.
    public class PaymentServices : IPaymentService
    {
        public bool BuyItems(Item item, int qty = 1)
        {
            return true;
        }
    }
}
