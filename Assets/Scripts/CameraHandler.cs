using UnityEngine;
using UnityEngine.InputSystem;

namespace WhiteJessica.PredatorPrey
{
    public class CameraHandler
    {
        // local reference camera switching component
        private CameraSwitcher cameraSwitcher;
        public Vector2 rotateInput;

        public CameraHandler(InputAction next, InputAction prev, InputAction rotate, InputAction zoomIn, InputAction zoomOut, CameraSwitcher cameraSwitcher)
        {
            // set local reference to camera switcher
            this.cameraSwitcher = cameraSwitcher;

            // subscribe methods to cameraSwitch input action
            next.performed += Next_performed;
            next.Enable();

            prev.performed += Prev_performed;
            prev.Enable();

            rotate.performed += Rotate_performed;
            rotate.Enable();

            zoomIn.performed += ZoomIn_performed;
            zoomIn.Enable();

            zoomOut.performed += ZoomOut_performed;
            zoomOut.Enable();
        }

        // event handlers linked to cameraSwitchers methods for rotation, zoom, and switching
        private void Next_performed(InputAction.CallbackContext obj) => this.cameraSwitcher.NextCamera();

        private void Prev_performed(InputAction.CallbackContext obj) => this.cameraSwitcher.PreviousCamera();

        private void Rotate_performed(InputAction.CallbackContext obj) => rotateInput = obj.ReadValue<Vector2>();

        private void ZoomIn_performed(InputAction.CallbackContext obj) => this.cameraSwitcher.ZoomIn();

        private void ZoomOut_performed(InputAction.CallbackContext obj) => this.cameraSwitcher.ZoomOut();
    }
}
