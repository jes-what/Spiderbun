using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    public class HungerState : AgentState
    {
        private AgentState wanderState;
        private SimulationManager simManager;
        private FoodPlant targetFood;

        public HungerState (Agent agent, AgentState wanderState) : base(agent) {
            this.wanderState = wanderState;
        }
        
        public override void Enter()
        {
            simManager = GameObject.FindFirstObjectByType<SimulationManager>();
            targetFood = null;
        }

        public override void Execute()
        {
            if (agent.currentEnergy >= agent.lowThreshold)
            {
                agent.ChangeState(wanderState);
                return;
            }

            // find food closest to agent
            if (targetFood == null || targetFood.IsDepleted)
            {
                targetFood = simManager.GetNearestFood(agent.transform.position);
            }

            if (targetFood == null) return;

            // call consume in FoodPlant
            float distance = Vector3.Distance(agent.transform.position, targetFood.transform.position);
            if (distance < 3f)
            {
                targetFood.Consume(agent);
            }

            // don't hit walls
            Vector3 direction = (targetFood.transform.position - agent.transform.position);
            if (agent.wallDetected)
            {
                float avoidanceStrength = agent.wallAvoidance.magnitude;
                agent.heading = (direction + agent.wallAvoidance * avoidanceStrength).normalized;
            } else
            {
                agent.heading = direction.normalized;
            }
        }

        public override void Exit() => targetFood = null;
    }
}
