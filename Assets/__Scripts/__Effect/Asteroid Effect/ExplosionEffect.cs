using UnityEngine;

namespace Course.Effect
{
    public class ExplosionEffect: Effect
    {
        [SerializeField]
        private ParticleSystem[] particleSystems;
        
        [Tooltip("If true, play exactly one random sub‐system; if false, play them all.")]
        [SerializeField]
        private bool playRandomly = false;

        public override void PlayEffect(float scaleFactor = 1f)
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                // No particle systems ⇒ immediately return to pool
                Release();
                return;
            }
            
            foreach (var ps in particleSystems)
            {
                ps.transform.localScale = Vector3.one * scaleFactor;
            }
            
            float maxDuration = 0f;

            if (playRandomly)
            {
                // Pick one random ParticleSystem index
                int randomIndex = Random.Range(0, particleSystems.Length);
                var ps = particleSystems[randomIndex];

                ps.Play();
                maxDuration = ps.main.duration;
            }
            else
            {
                // Play all ParticleSystems
                foreach (var ps in particleSystems)
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