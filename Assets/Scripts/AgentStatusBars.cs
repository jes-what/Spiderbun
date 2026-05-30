using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace WhiteJessica.PredatorPrey
{
    public class AgentStatusBars : MonoBehaviour
    {
        [SerializeField] private Image energyBarFill;
        [SerializeField] private Image hydrationBarFill;
        private CameraSwitcher cameraSwitcher;

        private Agent agent;
        void Start ()
        {
            agent = GetComponentInParent<Agent>();
            cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        }

        // determines how much of the light colored bars fill the sprite space
        void Update()
        {
            energyBarFill.fillAmount = agent.currentEnergy / agent.maxEnergy;
            hydrationBarFill.fillAmount = agent.currentHydration / agent.maxHydration;

            // point status bars at the active camera
            transform.LookAt(transform.position + cameraSwitcher.activeCamera.transform.forward);
        }
    }
}
