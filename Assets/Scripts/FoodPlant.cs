using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class FoodPlant : MonoBehaviour
    {
        [SerializeField] private GameObject fruitMesh;
        [SerializeField] private GameObject emptyBush;

        private float energyValue = 20f;
        private int maxFeedings = 2;
        private int feedingsRemaining;

        public bool IsDepleted => depleted;
        public float respawnTime = 15f;
        private float timer;
        private bool depleted = false;
        

        void Start() => feedingsRemaining = maxFeedings;

        // food bushes give diminishing amount of energy on subsequent feeds
        public void Consume(Agent agent)
        {
            if (agent.CompareTag("prey") && !depleted)
            {
                agent.GetComponent<Agent>().GainEnergy(energyValue);
                feedingsRemaining--;
                energyValue -= 10;
                if (feedingsRemaining <= 0) Deplete();
            }
        }

        // energy value of bush depletes, mimicking bunnies overeating
        private void Deplete()
        {
            depleted = true;
            timer = respawnTime;
            fruitMesh.SetActive(false);
            emptyBush.SetActive(true);
        }

        // once timer is up, reactivate fruit bush and deactivate empty bush
        void Update()
        {
            if (depleted)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    depleted = false;
                    feedingsRemaining = maxFeedings;
                    fruitMesh.SetActive(true);
                    emptyBush.SetActive(false);
                    energyValue = 20f;
                }
            }
        }
    }
}
