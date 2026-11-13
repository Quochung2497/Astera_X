namespace Course.Effect
{
    internal sealed class ExhaustTrailEmission
    {
        private const float MinSpeedSquared = 0.0001f;
        private float _framesRemaining;

        public void SuppressFor(float frames) => _framesRemaining = frames;

        public bool ConsumeSuppressedFrame()
        {
            if (_framesRemaining > 0f)
            {
                _framesRemaining--;
                return true;
            }

            return false;
        }

        public static bool ShouldEmit(float speedSquared) => speedSquared > MinSpeedSquared;
    }
}
