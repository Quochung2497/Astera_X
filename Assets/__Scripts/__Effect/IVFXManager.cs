using UnityEngine;

namespace Course.Effect
{
    public interface IVFXManager
    {
        void TriggerEffect(
            IParticlesEffectProvider particlesEffectContainer,
            Vector3 position,
            float scaleFactor = 1f
        );
    }
}
