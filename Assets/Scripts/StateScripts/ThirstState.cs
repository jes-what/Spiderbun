using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class ThirstState : AgentState
    {
        private AgentState wanderState;
        private SimulationManager simManager;
        private WaterSource targetWater;

        public ThirstState(Agent agent, AgentState wanderState) : base(agent)
        {
            this.wanderState = wanderState;
        }

        public override void Enter()
        {
            simManager = GameObject.FindFirstObjectByType<SimulationManager>();
            targetWater = simManager.GetNearestWater(agent.transform.position);
        }

        public override void Execute()
        {
            if (agent.currentHydration >= agent.lowThreshold)
            {
                agent.currentSpeed = agent.baseSpeed;
                agent.ChangeState(wanderState);
                return;
            }
            // try to keep agents out of the center of water sources
            float distance = Vector3.Distance(agent.transform.position, targetWater.transform.position);
            if (distance < 4.9f)
            {
                agent.currentSpeed = 0f;
                targetWater.Drink(agent);
                return;
            }

            // water sources don't deplete
            Vector3 direction = targetWater.transform.position - agent.transform.position;

            if(agent.wallDetected)
            {
                float avoidanceStrength = agent.wallAvoidance.magnitude;
                agent.heading = (direction.normalized + agent.wallAvoidance * avoidanceStrength).normalized;
            } else
            {
                agent.heading = direction.normalized;
            }
        }

        public override void Exit() => targetWater = null;
    }
}
