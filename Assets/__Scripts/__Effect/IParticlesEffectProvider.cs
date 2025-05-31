using System.Collections.Generic;
using UnityEngine;

namespace Course.Effect
{
    public interface IParticlesEffectProvider
    {
            void TriggerEffect(Vector3 position, float scaleFactor = 1f);
            IEnumerable<Effect> GetPrefabs();
    }
}

