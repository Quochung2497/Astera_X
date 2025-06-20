using UnityEngine;

namespace Course.Effect
{
    public class ExplosionEffect: Effect
    {
        [Tooltip("If true, play exactly one random sub‐system; if false, play them all.")]
        [SerializeField]
        private bool playRandomly = false;

        public override void PlayEffect(float scaleFactor = 1f)
        {
            if (particleSystems == null || particleSystems.Length == 0 || mobileParticleSystems == null || mobileParticleSystems.Length == 0)
            {
                // No particle systems ⇒ immediately return to pool
                Release();
                return;
            }
            
            foreach (var ps in CurrentSystems)
            {
                ps.transform.localScale = Vector3.one * scaleFactor;
            }
            
            float maxDuration = 0f;

            if (playRandomly)
            {
                // Pick one random ParticleSystem index
                int randomIndex = Random.Range(0, CurrentSystems.Length);
                var ps = CurrentSystems[randomIndex];

                ps.Play();
                maxDuration = ps.main.duration;
            }
            else
            {
                // Play all ParticleSystems
                foreach (var ps in CurrentSystems)
                {
                    ps.Play();
                    maxDuration = Mathf.Max(maxDuration, ps.main.duration);
                }
            }

            // After whichever system(s) finish, return to the pool
            StartCoroutine(ReleaseAfterDuration(maxDuration));
        }
    }
}