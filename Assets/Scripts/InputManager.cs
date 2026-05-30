using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private CameraSwitcher cameraSwitcher;
        private Inputs input;
        private CameraHandler cameraHandler;

        public Vector2 rotateInput;
        private bool initialized = false;

        private void Awake()
        {
            // create input asset and pass actions to the camera handler
            input = new Inputs();
            cameraHandler = new CameraHandler(
                    input.CameraSwitch.Next, 
                    input.CameraSwitch.Previous, 
                    input.CameraSwitch.Rotate,
                    input.CameraSwitch.ZoomIn,
                    input.CameraSwitch.ZoomOut,
                    cameraSwitcher
                    );
        }

        private void OnDisable() => input.CameraSwitch.Disable();

        private void Update()
        {
            // skip first frame to ensure all components are initialized
            if (!initialized)
            {
                initialized = true;
                return;
            }

            // pass rotate input to camera swicher each frame
            if (cameraHandler.rotateInput != Vector2.zero)
            {
                cameraSwitcher.RotateCamera(cameraHandler.rotateInput);
            }
        }
    }
}
