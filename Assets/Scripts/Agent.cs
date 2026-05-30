using System.Collections;
using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// Abstract base class for predator and prey agents. Handles movement, wall collision 
    /// response, and FSM state management. 
    /// </summary>
    public abstract class Agent : MonoBehaviour
    {
        protected SimulationManager simManager;
        protected AgentState currentState;
        private Animator animator;

        public Vector3 spawnPosition;
        public float homeRadius;

        // states need to know these values but must not be able to change them
        public float baseSpeed { get; protected set; }
        public float maxSpeed { get; protected set; }
        public float turnSpeed { get; protected set; }
        public float agentRadius { get; protected set; }

        // need to be public so states can read and change them
        public float currentSpeed;
        public Vector3 heading;
        public Vector3 wallAvoidance;
        public bool wallDetected;

        // energy and hydration system variables
        public float maxEnergy { get; protected set; }
        public float maxHydration { get; protected set; }
        public float currentEnergy { get; protected set; }
        public float currentHydration { get; protected set; }
        public float energyDrainRate { get; protected set; }
        public float hydrationDrainRate { get; protected set; }
        public float criticalThreshold { get; protected set; }
        public float lowThreshold { get; protected set; }

        // predator/prey agents implement these
        protected virtual void Init() { }
        protected virtual void Scan() { }

        // ensure Init() runs before state reset 
        void Awake()
        {
            simManager = FindFirstObjectByType<SimulationManager>();
            animator = GetComponentInChildren<Animator>();
            Init();
            // should call the TriggerAnimation script here
        }

        void FixedUpdate()
        {
            UpdateResources();
            Scan();
            currentState.Execute();
            Move();
            //sets the animation
            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed);
            }
        }

        // energy/hydration drain, die if either reaches 0
        protected void UpdateResources()
        {
            currentEnergy -= energyDrainRate * Time.fixedDeltaTime;
            currentHydration -= hydrationDrainRate * Time.fixedDeltaTime;

            if (currentEnergy <= 0f || currentHydration <= 0f)
            {
                Die(); // haha
            }
        }

        public void GainEnergy(float amount)
        {
            // add amount but don't exceed max energy
            currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        }

        public void GainHydration(float amount)
        {
            currentHydration = Mathf.Min(currentHydration + amount, maxHydration);
        }

        // moves the agent along current heading, applies wall push to keep agents away from wall surfaces
        protected virtual void Move()
        {
            // this is why agents are not avoiding water and food sources
            // checking states in here would break encapsulation
            // fawk, move should be in each agent's script
            int wallMask = LayerMask.GetMask("Obstacles");
            Vector3 totalPush = Vector3.zero;

            Collider[] collisions = Physics.OverlapSphere(
                transform.position, 
                agentRadius, 
                wallMask
             );

            // push direction away from nearest point on the collider, scale push strength by degree of penetration into the radius
            foreach (Collider col in collisions)
            {
                Vector3 pushDirection = (transform.position - col.ClosestPoint(transform.position));
                pushDirection.y = 0f;
                if (pushDirection.magnitude > 0.001f)
                {
                    totalPush += pushDirection.normalized * (agentRadius - pushDirection.magnitude);
                }   
            }
            heading.y = 0f;
            heading = heading.normalized;

            float moveDistance = currentSpeed * Time.fixedDeltaTime;

            // restrict move distance if a wall would be reached in this timestep
            if (Physics.Raycast(transform.position, heading, out RaycastHit hit, moveDistance + agentRadius, wallMask))
            {
                moveDistance = Mathf.Max(0f, hit.distance - agentRadius);
            }

            transform.position += totalPush + heading * moveDistance;
            transform.forward = heading;
        }

        protected internal void Die()
        {
            // stop Die from being called twice
            if (!enabled) return;
            if (animator != null)
            {
                animator.SetTrigger("Dead");
            }
            currentSpeed = 0f;
            // stop fixed update
            enabled = false;
            StartCoroutine(DeathDelay());
        }

        private IEnumerator DeathDelay()
        {
            yield return new WaitForSeconds(5f);
            simManager.RespawnAgent(gameObject);
        }

        // we literally don't call this anywhere at all
        public void TriggerAnimation(string trigger)
        {
            if (animator != null)
            {
                animator.SetTrigger(trigger);
            }
        }

        // transition the FSM to new state, safely exiting this one and entering the next 
        public void ChangeState(AgentState newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

    }
}
