 using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// State dictating flee behavior for prey agents. Transitions back to wander state if the 
    /// predator is lost or moves out of range. Coupled to prey.
    /// </summary>
    public class FleeState : AgentState
    {
        // wall tangent direction prey will be sliding against
        private Vector3 slideTo = Vector3.zero;
        private bool sliding = false;

        private AgentState wanderState;

        public FleeState(Agent agent, AgentState wanderState) : base(agent) {
            this.wanderState = wanderState;
        }

        public override void Enter() => agent.currentSpeed = agent.maxSpeed;

        // steers prey away from detected predator
        public override void Execute()
        {
            Prey prey = agent as Prey;
            if (prey.detectedPredator == null || Vector3.Distance(agent.transform.position, prey.detectedPredator.transform.position) > 5f)
            {
                agent.ChangeState(wanderState);
                return;
            }

            Vector3 fleeDirection = (agent.transform.position - prey.detectedPredator.transform.position).normalized;
            
            if (agent.wallDetected)
            {
                // determine both tangent directions along wall surface
                Vector3 wallNormal = agent.wallAvoidance.normalized;
                Vector3 tangentA = Vector3.Cross(wallNormal, Vector3.up).normalized;
                Vector3 tangentB = -tangentA;

                if (!sliding)
                {
                    sliding = true;
                    slideTo = (Vector3.Dot(agent.heading, tangentA) > Vector3.Dot(agent.heading, tangentB)) 
                        ? tangentA 
                        : tangentB;
                }
                // rotate toward the chosen slide direction
                agent.heading = Vector3.RotateTowards(
                    agent.heading, 
                    slideTo, 
                    agent.turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 
                    0f
                 );
            } else
            {
                // reset slide and resume fleeing 
                sliding = false;
                agent.heading = fleeDirection;
            }
        }

        public override void Exit()
        {
            agent.currentSpeed = agent.baseSpeed;
            (agent as Prey).detectedPredator = null;
        }
    }
}
