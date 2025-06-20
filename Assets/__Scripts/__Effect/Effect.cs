using System.Collections;
using System.Linq;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Effect
{
    public abstract class Effect : MonoBehaviour, IPoolObject<Effect>
    {
        [SerializeField]
        protected ParticleSystem[] particleSystems;

        [SerializeField] 
        protected ParticleSystem[] mobileParticleSystems;
        
        #if UNITY_EDITOR
        [SerializeField]
        protected bool isMobilePlatform = false; // for editor testing only
        #endif        
        private IObjectPool<Effect> _pool;
        
        protected ParticleSystem[] CurrentSystems
        {
            get
            {
                // note: Application.isMobilePlatform covers iOS & Android at runtime
                if (Application.isMobilePlatform 
                    && mobileParticleSystems != null 
                    && mobileParticleSystems.Length > 0)
                    return mobileParticleSystems;
#if UNITY_EDITOR
                if(isMobilePlatform 
                   && mobileParticleSystems != null 
                   && mobileParticleSystems.Length > 0)
                    return mobileParticleSystems;
#endif
                return particleSystems;
            }
        }
        
        protected virtual void Awake()
        {
            var keep = CurrentSystems ?? new ParticleSystem[0];

            void Disable(ParticleSystem ps)
            {
                if (ps == null) return;
                if (!keep.Contains(ps))
                    ps.gameObject.SetActive(false);
            }

            if (particleSystems != null)
                foreach (var ps in particleSystems) Disable(ps);

            if (mobileParticleSystems != null)
                foreach (var ps in mobileParticleSystems) Disable(ps);
        }

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

