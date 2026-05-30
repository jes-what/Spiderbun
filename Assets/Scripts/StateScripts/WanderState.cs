using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// State dictating wander behavior for both predator and prey agents. Transitions to chase or flee
    /// triggered by each agent's scan method when a target is detected.
    /// </summary>
    public class WanderState : AgentState
    {
        // current angle & distance wander circle projected
        private float wanderAngle;
        private float wanderDist = 2f;

        // radius and jitter of wander circle
        private float wanderRadius = 1f;
        private float wanderJitter = 10f;

        private float wanderResetTimer = 0f;
        private float wanderResetInterval = 10f;

        public WanderState(Agent agent) : base(agent) { }

        public override void Enter()
        {
            // pick a random wander angle and heading
            wanderAngle = Random.Range(0f, 360f);
            agent.heading = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        }

        public override void Execute()
        {
            // wander reset randomly changes movement direction
            wanderResetTimer += Time.fixedDeltaTime;
            if(wanderResetTimer >= wanderResetInterval)
            {
                wanderAngle = Random.Range(-90f, 90f);
                wanderResetTimer = 0f;
            }

            wanderAngle += Random.Range(-wanderJitter, wanderJitter);

            //project circle ahead of agent and compute target point on its edge
            Vector3 circleCenter = agent.heading * wanderDist;
            Vector3 wanderTarget = circleCenter + new Vector3(
                Mathf.Sin(wanderAngle * Mathf.Deg2Rad) * wanderRadius, 
                0f,
                Mathf.Cos(wanderAngle * Mathf.Deg2Rad) * wanderRadius
                );

            if (agent.wallDetected)
            {
                // combine current heading with wall avoidance to steer away
                Vector3 desired = (agent.heading + agent.wallAvoidance).normalized;
                agent.heading = desired;
                return;
            }

            // rotate naturally toward wander target restricted by turn speed
            agent.heading = Vector3.RotateTowards(
                agent.heading,
                wanderTarget.normalized,
                agent.turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0f
                );
            
        }

        public override void Exit() { }
    }
}
