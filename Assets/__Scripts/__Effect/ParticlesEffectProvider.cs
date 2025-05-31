using System.Collections.Generic;
using UnityEngine;

namespace Course.Effect
{
    [CreateAssetMenu(fileName = "ParticlesEffectProvider", menuName = "Scriptable Objects/ParticlesEffectProvider")]
    public class ParticlesEffectProvider : UnityEngine.ScriptableObject, IParticlesEffectProvider
    {
        [SerializeField]
        private Effect[] effects = null;

        public void TriggerEffect(Vector3 position, float scaleFactor = 1f)
        {
            var _vfxManager = VFXManager.TryGetInstance();
            _vfxManager.TriggerEffect(this, position, scaleFactor);
        }

        public IEnumerable<Effect> GetPrefabs()
        {
            foreach (var effect in effects)
            {
                yield return effect;
            }
        }
    }
}

