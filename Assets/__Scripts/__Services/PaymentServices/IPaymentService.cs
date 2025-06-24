using Course.ScriptableObject.Item;

namespace Course.Services
{
    public interface IPaymentService
    {
        bool BuyItems(Item item, int qty = 1);
    }
}