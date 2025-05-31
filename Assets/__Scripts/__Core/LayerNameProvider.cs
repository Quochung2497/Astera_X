using UnityEngine;

namespace Course.Core
{
    /// <summary>
    /// Provides utility methods for working with layer names in Unity.
    /// </summary>
    public static class LayerNameProvider
    {
        /// <summary>
        /// Retrieves the layer index corresponding to the specified <see cref="LayerName"/>.
        /// </summary>
        /// <param name="layerName">The layer name to retrieve the index for.</param>
        /// <returns>The index of the layer.</returns>
        public static int GetLayer(LayerName layerName)
        {
            return LayerMask.NameToLayer(layerName.ToString());
        }
    }
}

/// <summary>
/// Enum representing predefined layer names in the game.
/// </summary>
public enum LayerName
{
    /// <summary>
    /// The layer assigned to the player.
    /// </summary>
    Player,

    /// <summary>
    /// The layer assigned to enemies.
    /// </summary>
    Enemy,

    /// <summary>
    /// The layer assigned to asteroids.
    /// </summary>
    Asteroid,
}