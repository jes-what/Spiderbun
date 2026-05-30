using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// Prey agent. Initializes prey objects and states, enables disabled prey, scans for
    /// walls and predators, and flees if predator detected.
    /// </summary>
    public class Prey : Agent
    {
        // closest visible predator
        public GameObject detectedPredator;

        private WanderState wanderState;
        private FleeState fleeState;
        private HungerState hungerState;
        private ThirstState thirstState;

        // sets prey-specific agent values and initializes FSM
        protected override void Init()
        {
            baseSpeed = 5f;
            maxSpeed = 7.5f;
            turnSpeed = 150f;
            agentRadius = 1f;

            currentSpeed = baseSpeed;

            maxEnergy = 80f;
            maxHydration = 80f;
            currentEnergy = maxEnergy;
            currentHydration = maxHydration;
            energyDrainRate = 1.2f;
            hydrationDrainRate = 2f;
            criticalThreshold = 20f;
            lowThreshold = 50f;

            wanderState = new WanderState(this);
            fleeState = new FleeState(this, wanderState);
            hungerState = new HungerState(this, wanderState);
            thirstState = new ThirstState(this, wanderState);

            currentState = wanderState;
            currentState.Enter();
        }

        // resets prey to a clean wander state on re-enable after respawn
        void OnEnable()
        {
            detectedPredator = null;
            Init();
        }

        protected override void Scan()
        {
            // avoid water and food sources unless in thirst or hunger state
            int wallMask = LayerMask.GetMask("Obstacles");
            if (!(currentState is HungerState))
            {
                wallMask |= LayerMask.GetMask("Food");
            }
            if (!(currentState is ThirstState))
            {
                wallMask |= LayerMask.GetMask("Water");
            }

            ScanWalls(wallMask);
            ScanForPredators(wallMask);

            if (currentHydration < criticalThreshold && !(currentState is ThirstState) && !(currentState is HungerState))
            {
                ChangeState(thirstState);
                return;
            }

            if (currentEnergy < criticalThreshold && !(currentState is HungerState) && !(currentState is ThirstState))
            {
                ChangeState(hungerState);
                return;
            }
        }

        private void ScanWalls(int wallMask)
        {
            wallAvoidance = Vector3.zero;
            wallDetected = false;

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
            // only change wall detected if avoidance is worth doing so
            wallDetected = wallAvoidance.magnitude > 0.1f;
        }

        private void ScanForPredators(int wallMask)
        {
            float detectionRange = 5f;

            float closestPredator = float.MaxValue;
            detectedPredator = null;

            GameObject[] predatorObjects = GameObject.FindGameObjectsWithTag("predator");
            foreach (GameObject p in predatorObjects)
            {
                Vector3 directionToPredator = p.transform.position - transform.position;
                float distance = directionToPredator.magnitude;
                directionToPredator.Normalize();

                // check if predator within range, obj's blocking LOS, track only closest visible predator 
                if (distance < detectionRange)
                {
                    if (!Physics.Raycast(transform.position, directionToPredator, distance, wallMask))
                    {
                        // this is part of the problem
                        if (distance < closestPredator)
                        {
                            closestPredator = distance;
                            detectedPredator = p;
                        }
                    }
                }
            }

            if (detectedPredator != null && !(currentState is FleeState))
            {
                ChangeState(fleeState);
            }
        }
    }
}
