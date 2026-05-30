namespace WhiteJessica.PredatorPrey
{
    /// <summary>
    /// Agent has an AgentState. AgentState has a reference to Agent so it can modify heading.
    /// </summary>
    public abstract class AgentState
    {
        protected Agent agent;

        public AgentState(Agent agent)
        {
            this.agent = agent;
        }

        // Concrete state classes extending this class must implement these methods.
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}
