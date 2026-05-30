using Unity.VisualScripting;
using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// Predator agent. Initializes predator objects and states, scans for
    /// walls and prey, and chases if prey detected.
    /// </summary>
    public class Predator : Agent
    {
        // closest visible prey
        public GameObject detectedPrey;

        private WanderState wanderState;
        private ChaseState chaseState;
        private ThirstState thirstState;

        private bool isHungry;
        
        // set predator-specific agent values and initialize FSM
        protected override void Init()
        { 
            baseSpeed = 5f;
            maxSpeed = 8f;
            turnSpeed = 120f;
            agentRadius = 1.5f;
            currentSpeed = baseSpeed;

            // energy system starting values
            maxEnergy = 100f;
            maxHydration = 100f;
            energyDrainRate = 3f;
            hydrationDrainRate = 2f;
            currentEnergy = maxEnergy;
            currentHydration = maxHydration;

            criticalThreshold = 30f;
            lowThreshold = 60f;

            // all states for this agent
            wanderState = new WanderState(this);
            chaseState = new ChaseState(this, wanderState);
            thirstState = new ThirstState(this, wanderState);

            currentState = wanderState;
            currentState.Enter();
        }

        void OnEnable()
        {
            detectedPrey = null;
            Init();
        }

        protected override void Scan()
        {
            int wallMask = LayerMask.GetMask("Obstacles");
            wallMask |= LayerMask.GetMask("Food"); // still needs to avoid bushes always, check not needed
            if (!(currentState is ThirstState))
            {
                wallMask |= LayerMask.GetMask("Water");
            }

            ScanWalls(wallMask);
            
            // prioritize hydration over food
            if (currentHydration < criticalThreshold && !(currentState is ThirstState) && !(currentState is HungerState))
            {
                ChangeState(thirstState);
                return;
            }

            // only bother prey when hungry
            isHungry = currentEnergy < lowThreshold;
            if (isHungry && !(currentState is ThirstState) && !(currentState is HungerState))
            {
                ScanForPrey(wallMask);
            }
        }


        // updates wall avoidance data
        private void ScanWalls(int wallMask)
        {
            wallAvoidance = Vector3.zero;
            wallDetected = false;

            // scale scan radius with currentSpeed (1 < means chasing)
            float scanRadius = 3f * (currentSpeed / baseSpeed);
            Collider[] colliders = Physics.OverlapSphere(transform.position, scanRadius, wallMask);
            foreach (Collider col in colliders)
            {
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                Vector3 steerDirection = transform.position - closestPoint;
                steerDirection.y = 0f;

                float dist = steerDirection.magnitude;
                if (dist < 0.001f) continue;

                Vector3 towardObstacle = -steerDirection.normalized;
                float forwardWeight = Mathf.Max(0f, Vector3.Dot(towardObstacle, heading));

                wallAvoidance += steerDirection.normalized * (1f - dist / scanRadius) * (forwardWeight + 0.3f);
            }

            // only set wall detected to true if avoidance worth it
            wallDetected = wallAvoidance.magnitude > 0.1f;
        }


        // scans for prey within detection range
        private void ScanForPrey(int wallMask)
        {
            if (currentState is ChaseState && detectedPrey != null) return;
            float detectionRange = 30f;

            // fov widens when chasing to simulate heightened focus
            float fovAngle = currentState is ChaseState ? 150f : 90f;
            float cosVisionLimit = Mathf.Cos(fovAngle * 0.5f * Mathf.Deg2Rad);

            float closestPrey = float.MaxValue;
            GameObject closestDetected = null;

            GameObject[] preyObjects = GameObject.FindGameObjectsWithTag("prey");
            foreach (GameObject p in preyObjects)
            {
                Vector3 directionToPrey = p.transform.position - transform.position;
                float distance = directionToPrey.magnitude;
                directionToPrey.Normalize();

                float dot = Vector3.Dot(directionToPrey, heading);

                // check if prey within range inside the cone, obstacle blocking LOS, track closest prey
                if (distance < detectionRange && dot > cosVisionLimit)
                {
                    if (!Physics.Raycast(transform.position, directionToPrey, distance, wallMask))
                    {
                        if (distance < closestPrey)
                        {
                            closestPrey = distance;
                            closestDetected = p;
                        }
                    }
                }
            }
            detectedPrey = closestDetected;

            if (detectedPrey != null && !(currentState is ChaseState))
            {
                ChangeState(chaseState);
            }
        }
    }
}
