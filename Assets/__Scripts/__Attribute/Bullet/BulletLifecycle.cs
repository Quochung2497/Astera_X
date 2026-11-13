using System;

namespace Course.Attribute.Bullet
{
    internal sealed class BulletLifecycle
    {
        private enum Phase
        {
            Reserved,
            Launched,
            Released
        }

        private Phase _phase = Phase.Reserved;

        public bool IsLaunched => _phase == Phase.Launched;
        public Guid ShotId { get; private set; }

        public void Reserve() => _phase = Phase.Reserved;

        public void Launch()
        {
            ShotId = Guid.NewGuid();
            _phase = Phase.Launched;
        }

        public void ResetLaunch()
        {
            if (IsLaunched)
                _phase = Phase.Reserved;
        }

        public bool TryRelease()
        {
            if (_phase == Phase.Released)
                return false;

            _phase = Phase.Released;
            return true;
        }
    }
}
