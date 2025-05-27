using UnityEngine.Pool;

namespace Course.Utility
{
  public interface IPool<T> where T : class
  {
    IObjectPool<T> GetPool();
  }
}
