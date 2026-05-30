using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// State dictating chase behavior for predator agents. Transitions back to wander state
    /// if prey is lost for too long or consumed. Coupled with predator.
    /// </summary>
    public class ChaseState : AgentState
    {
        // how long predator has been unable to see prey & time interval before giving up
        private float lostPreyTimer = 0f;
        private float lostPreyDuration = 1.5f;
        private float energyValue = 30f;

        private AgentState wanderState;

        public ChaseState(Agent agent, AgentState wanderState) : base(agent) {
            this.wanderState = wanderState;
        }

        public override void Enter() => agent.currentSpeed = agent.maxSpeed;

        public override void Execute()
        {
            Predator predator = agent as Predator;

            // count up and return to wander if prey lost for long enough
            if (predator.detectedPrey == null)
            {
                lostPreyTimer += Time.fixedDeltaTime;
                if(lostPreyTimer >= lostPreyDuration)
                {
                    lostPreyTimer = 0f;
                    agent.ChangeState(wanderState);
                }
                return;
            }
            //reset timer when prey visible
            lostPreyTimer = 0f;

            Vector3 chaseDirection = (predator.detectedPrey.transform.position - agent.transform.position);
            float distance = chaseDirection.magnitude;

            // consume prey if within reach, call Die() on prey, give pred. energy, reset 
            if (distance < 1.5f)
            { 
                predator.detectedPrey.GetComponent<Prey>().Die();
                agent.GainEnergy(energyValue);
                predator.detectedPrey = null;
                agent.currentSpeed = agent.baseSpeed;
                agent.ChangeState(wanderState);
                return;
            }

            if (agent.wallDetected)
            {
                // scales the influence so closer walls push harder
                float avoidanceStrength = agent.wallAvoidance.magnitude;
                agent.heading = (chaseDirection.normalized + agent.wallAvoidance * avoidanceStrength).normalized;
            } else
            {
                agent.heading = chaseDirection.normalized;
            }
        }

        public override void Exit()
        {
            agent.currentSpeed = agent.baseSpeed;
            (agent as Predator).detectedPrey = null;
        }
    }
}
