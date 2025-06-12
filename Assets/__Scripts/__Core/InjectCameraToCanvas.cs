using UnityEngine;

namespace Course.Core
{
    [DefaultExecutionOrder(-100)]
    public class InjectCameraToCanvas : MonoBehaviour
    {
        /// <summary>
        /// Reference to the main camera in the scene.
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// Unity's Awake method.
        /// Initializes the camera reference and injects it into canvases with ScreenSpaceCamera render mode.
        /// </summary>
        private void Awake()
        {
            // Assign the main camera to the _camera field.
            _camera = Camera.main;

            // Inject the camera into all relevant canvases.
            InjectCameraIntoCanvases();
        }

        /// <summary>
        /// Injects the main camera into all canvases in the scene that use ScreenSpaceCamera render mode.
        /// </summary>
        private void InjectCameraIntoCanvases()
        {
            // Iterate through all Canvas objects in the scene.
            foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                // Check if the canvas render mode is ScreenSpaceCamera.
                if (c.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    // Assign the main camera to the canvas's worldCamera property.
                    c.worldCamera = _camera;
                }
            }
        }
    }

}
