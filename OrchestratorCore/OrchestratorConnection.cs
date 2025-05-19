namespace OrchestratorCore
{
    public class OrchestratorConnection
    {
        public OrchestratorWorkNode SourceNode;
        public OrchestratorWorkNode ReceivingNode;

        private bool enabled = false;

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public OrchestratorConnection(OrchestratorWorkNode sourceNode, OrchestratorWorkNode receivingNode)
        {
            SourceNode = sourceNode;
            ReceivingNode = receivingNode;

            SourceNode.SolutionEnd += (o, e) =>
            {
                if (Enabled)
                {
                    ReceivingNode.RequestSolution();
                }
            };
        }
    }
}