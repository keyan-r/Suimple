using OrchestratorCore;

namespace OrchestratorRhinoCore
{
    public class OrchestratorGHNodeLink : OrchestratorConnection
    {

        public OrchestratorGHNodeLink(OrchestratorGHNode parentNode, OrchestratorGHNode childNode) : base(parentNode, childNode)
        {
            //ParentNode = parentNode;
            //ChildNode = childNode;

            //Connection = new OrchestratorConnection(ParentNode, ChildNode);
        }
    }
}
