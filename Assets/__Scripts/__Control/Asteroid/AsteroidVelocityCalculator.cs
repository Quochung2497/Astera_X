using UnityEngine;

namespace Course.Control.Asteroid
{
    internal static class AsteroidVelocityCalculator
    {
        private const float InwardTargetRadius = 4f;

        public static VelocityInfo Calculate(
            Vector3 currentPosition, int size, bool isOutOfBounds,
            float minSpeed, float maxSpeed, float maxAngularSpeed)
        {
            Vector3 direction;
            if (isOutOfBounds)
            {
                direction = (Vector3)Random.insideUnitCircle * InwardTargetRadius - currentPosition;
                direction.Normalize();
            }
            else
            {
                do
                {
                    direction = Random.insideUnitCircle;
                    direction.Normalize();
                }
                while (Mathf.Approximately(direction.magnitude, 0f));
            }

            float speed = Random.Range(minSpeed, maxSpeed) / size;
            return new VelocityInfo
            {
                Linear = direction * speed,
                Angular = Random.insideUnitSphere * maxAngularSpeed
            };
        }
    }
}
