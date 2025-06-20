using UnityEngine;

namespace Course.Effect
{
    public class DisappearEffect : Effect
    {
        public override void PlayEffect(float scaleFactor = 1f)
        {
            float maxDuration = 0f;

            // Play all ParticleSystems
            foreach (var ps in CurrentSystems)
            {
                ps.Play();
                maxDuration = Mathf.Max(maxDuration, ps.main.duration);
            }
            
            // After whichever system(s) finish, return to the pool
            StartCoroutine(ReleaseAfterDuration(maxDuration));
        }
    }
}
