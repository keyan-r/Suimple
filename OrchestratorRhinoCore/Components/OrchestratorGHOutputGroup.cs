using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace OrchestratorRhinoCore.Components
{
    public class OrchestratorGHOutputGroup : OrchestratorGHGroup
    {
        public GH_Document GH_Document;        

        public OrchestratorGHOutputGroup(GH_Group ghGroup) : base(ghGroup)
        {
            GH_Document = ghGroup.OnPingDocument();
        }

        //public void UpdateValues(object sender, GH_SolutionEventArgs e)
        //{
        //    foreach (var component in Components)
        //    {
        //        component.GetValueFromGH();
        //    }
        //}
    }
}