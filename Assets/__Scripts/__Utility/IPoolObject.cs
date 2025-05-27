using UnityEngine.Pool;

namespace Course.Utility
{
  public interface IPoolObject<T> where T : class
  {
    void SetPool(IObjectPool<T> pool);
    void Reset();
    void Release();
  }
}
