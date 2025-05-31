using System.Collections.Generic;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Effect
{
    public class VFXManager : PrivateSingleton<VFXManager>, IVFXManager
    {
        // Constants
        private const int PoolDefaultCapacity = 5;
        private const int PoolSize = 10;

        // Private
        private readonly Dictionary<IParticlesEffectProvider, IObjectPool<Effect>[]> _effectPools =
            new();

        public void TriggerEffect(IParticlesEffectProvider particlesEffectProvider, Vector3 position, float scaleFactor = 1f)
        {
            if (!_effectPools.ContainsKey(particlesEffectProvider))
            {
                var prefabs = particlesEffectProvider.GetPrefabs();
                List<IObjectPool<Effect>> pools = new();
                foreach (var prefab in prefabs)
                {
                    pools.Add(
                        new ObjectPool<Effect>(
                            () => OnCreate(prefab),
                            OnGet,
                            OnRelease,
                            OnDestroyObject,
                            true,
                            PoolDefaultCapacity,
                            PoolSize
                        )
                    );
                }
                _effectPools.Add(particlesEffectProvider, pools.ToArray());
            }

            foreach (var pool in _effectPools[particlesEffectProvider])
            {
                var instance = pool.Get();
                instance.SetPool(pool);
                instance.transform.position = position;
                instance.PlayEffect(scaleFactor);
            }
        }

        // Private
        private Effect OnCreate(Effect prefab)
        {
            var instance = Instantiate(prefab);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnGet(Effect instance)
        {
            instance.gameObject.SetActive(true);
        }

        private void OnRelease(Effect instance)
        {
            instance.gameObject.SetActive(false);
        }

        private void OnDestroyObject(Effect instance)
        {
            Destroy(instance);
        }
    }
}
