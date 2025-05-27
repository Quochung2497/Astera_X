using UnityEngine;

namespace Course.Core
{
    public interface IFactory<T>
    {
        T Get();
    }
}
