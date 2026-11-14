using System.Collections.Generic;
using Course.Core;

namespace Course.Control
{
    internal sealed class ShipPartAvailability
    {
        private readonly HashSet<int> _unlockedAchievements = new();
        private readonly HashSet<(PartType, int)> _purchasedParts = new();

        public void UnlockAchievement(int achievementId) => _unlockedAchievements.Add(achievementId);

        public void RecordPurchase(PartType type, int index) => _purchasedParts.Add((type, index));

        public bool CanSelect(PartType type, int index, int? requirement) =>
            !requirement.HasValue || _unlockedAchievements.Contains(requirement.Value) ||
            _purchasedParts.Contains((type, index));
    }
}
