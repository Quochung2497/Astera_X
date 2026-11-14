using System.Collections.Generic;
using Course.Core;

namespace Course.Control
{
    internal sealed class ShipPartSelection<TPart>
    {
        private readonly IReadOnlyDictionary<(PartType, int), TPart> _catalog;

        public ShipPartSelection(IReadOnlyDictionary<(PartType, int), TPart> catalog)
        {
            _catalog = catalog;
        }

        public int SelectedBodyIndex { get; private set; }
        public int SelectedTurretIndex { get; private set; }

        public bool TryGetPart(PartType type, int index, out TPart part) =>
            _catalog.TryGetValue((type, index), out part);

        public void SetSelectedIndex(PartType type, int index)
        {
            if (type == PartType.Body)
                SelectedBodyIndex = index;
            else
                SelectedTurretIndex = index;
        }

        public void RestoreIndices(int bodyIndex, int turretIndex)
        {
            SelectedBodyIndex = bodyIndex;
            SelectedTurretIndex = turretIndex;
        }
    }
}
