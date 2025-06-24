using Course.Services;
using UnityEngine;

namespace Course.Attribute
{
    // TODO: Temporary class to test Purse functionality, will be replaced with a proper implementation later.
    public class PurseBehaviour : MonoBehaviour,IPurse
    {
        public bool CanAfford(int amount)
        {
            return true;
        }

        public bool Spend(int amount)
        {
            return true;
        }
    }
}
