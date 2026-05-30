using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class WaterSource : MonoBehaviour
    {
        public float hydrationValue = 10f;
        // one method, probably not worth a script, but I had bigger plans
        public void Drink(Agent agent) => agent.GainHydration(hydrationValue * Time.fixedDeltaTime);
    }
}
