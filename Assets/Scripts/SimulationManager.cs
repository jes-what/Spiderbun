using System.Collections;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    // I want to move general spawning out of here, and move "breeding" into AgentSpawner (rename it as well)
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private Transform predatorParent;
        [SerializeField] private GameObject predatorPrefab;

        [SerializeField] private Transform preyParent;
        [SerializeField] private GameObject preyPrefab;

        [SerializeField] private int initialPredators = 10;
        [SerializeField] private int initialPrey = 20;

        private Vector3 predatorSpawnPoint;
        private Vector3 preySpawnPoint;

        private FoodPlant[] foodPlants;
        private WaterSource[] waterSources;

        void Start()
        {
            predatorSpawnPoint = new Vector3(-33f, 0f, 32f);
            preySpawnPoint = new Vector3(42f, 0f, -39f);

            SpawnAgents(predatorPrefab, initialPredators, predatorSpawnPoint, predatorParent);
            SpawnAgents(preyPrefab, initialPrey, preySpawnPoint, preyParent);

            // water and energy sources
            foodPlants = FindObjectsByType<FoodPlant>(FindObjectsSortMode.None);
            waterSources = FindObjectsByType<WaterSource>(FindObjectsSortMode.None);
        }

        private void SpawnAgents(GameObject prefab, int count, Vector3 center, Transform parent)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject agent = Instantiate(prefab, center, Quaternion.identity, parent);
                Agent agentScript = agent.GetComponent<Agent>();
                agentScript.spawnPosition = center;
            }
        }

        public FoodPlant GetNearestFood(Vector3 position)
        {
            FoodPlant nearest = null;
            float closestPlant = float.MaxValue;
            foreach (FoodPlant plant in foodPlants)
            {
                if (plant.IsDepleted) continue;
                float dist = Vector3.Distance(position, plant.transform.position);
                if (dist < closestPlant)
                {
                    closestPlant = dist;
                    nearest = plant;
                }
            }
            return nearest;
        }

        public WaterSource GetNearestWater(Vector3 position)
        {
            WaterSource nearest = null;
            float closestWater = float.MaxValue;
            foreach (WaterSource water in waterSources)
            {
                float dist = Vector3.Distance(position, water.transform.position);
                if (dist < closestWater)
                {
                    closestWater = dist;
                    nearest = water;
                }
            }
            return nearest;
        }


        private float respawnDelay = 5f;

        // starts respawn coroutine, needs to spawn both agents, take spawnPosition arg
        public void RespawnAgent(GameObject agent) => StartCoroutine(Respawn(agent));

        // disables prey on death, waits, repositions and re-enables it, OnEnable in Prey handles FSM reset
        private IEnumerator Respawn(GameObject agent)
        {
            agent.SetActive(false);
            yield return new WaitForSeconds(respawnDelay + Random.Range(0f, 5f));
            Vector3 offset = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            agent.transform.position = agent.GetComponent<Agent>().spawnPosition + offset;
            Agent agentScript = agent.GetComponent<Agent>();
            agentScript.enabled = true;
            agent.SetActive(true);
        }

    }
}
