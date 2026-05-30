using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private Camera[] cameras;
        [SerializeField] private Camera defaultCamera;
        public Camera activeCamera;

        private float xAxRot = 0f;
        private float yAxRot = 0f;
        private float rotateSpeed = 15f;

        private float zoomSpeed = 1f;
        private float minFOV = 5f;
        private float maxFOV = 80f;
        

        private int index;
        
        void Start()
        {
            // current camera
            index = 0;

            // loop through each camera and disable it
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].enabled = false;
            }

            // enable the default camera
            defaultCamera.enabled = true;
            activeCamera = defaultCamera;

            xAxRot = cameras[0].transform.eulerAngles.x;
            yAxRot = cameras[0].transform.eulerAngles.y;
        }

        // Update is called once per frame
        public void NextCamera()
        {
            // capture current index, and change index to next camera's
            int prev = index;
            index = (index + 1) % cameras.Length;

            // enable current and disable previous camera
            cameras[index].enabled = true;
            cameras[prev].enabled = false;
            activeCamera = cameras[index];
            xAxRot = cameras[index].transform.eulerAngles.x;
            yAxRot = cameras[index].transform.eulerAngles.y;
        }

        public void PreviousCamera()
        {
            int next = index;
            index = ((index - 1) + cameras.Length) % cameras.Length;
            cameras[next].enabled = false;
            cameras[index].enabled = true;
            activeCamera = cameras[index];
            xAxRot = cameras[index].transform.eulerAngles.x;
            yAxRot = cameras[index].transform.eulerAngles.y;
        }

        public void RotateCamera(Vector2 input)
        {
            xAxRot -= input.y * rotateSpeed * Time.deltaTime;
            xAxRot = Mathf.Clamp(xAxRot, -25f, 90f);
            yAxRot = cameras[index].transform.eulerAngles.y + input.x * rotateSpeed * Time.deltaTime;
            cameras[index].transform.rotation = Quaternion.Euler(xAxRot, yAxRot, 0f);
        }

        public void ZoomIn()
        {
            cameras[index].fieldOfView -= zoomSpeed;
            cameras[index].fieldOfView = Mathf.Clamp(cameras[index].fieldOfView, minFOV, maxFOV);
        }

        public void ZoomOut()
        {
            cameras[index].fieldOfView += zoomSpeed;
            cameras[index].fieldOfView = Mathf.Clamp(cameras[index].fieldOfView, minFOV, maxFOV);
        }
    }
}
