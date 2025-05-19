using System.Collections.Generic;

namespace OrchestratorRhinoCore
{
    public class OrchestratorGHSequence
    {
        public List<OrchestratorGHNode> Nodes { get; set; }

        public List<OrchestratorGHNodeLink> Links { get; set; }

        public OrchestratorGHSequence()
        {
            Nodes = new List<OrchestratorGHNode>();
            Links = new List<OrchestratorGHNodeLink>();
        }
    }
}