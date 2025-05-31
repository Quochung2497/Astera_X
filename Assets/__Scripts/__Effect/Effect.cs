using System.Collections;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Effect
{
    public abstract class Effect : MonoBehaviour, IPoolObject<Effect>
    {
        private IObjectPool<Effect> _pool;

        public abstract void PlayEffect(float scaleFactor = 1f);

        public void SetPool(IObjectPool<Effect> pool)
        {
            _pool = pool;
        }

        public void Reset() { }

        public void Release()
        {
            _pool.Release(this);
        }

        protected IEnumerator ReleaseAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            Release();
        }
    }
}

