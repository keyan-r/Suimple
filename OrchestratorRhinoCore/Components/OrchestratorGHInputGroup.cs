using System;
using Grasshopper.Kernel.Special;

namespace OrchestratorRhinoCore.Components
{
    public class OrchestratorGHInputGroup : OrchestratorGHGroup
    {
        public EventHandler DocumentSolutionRequested;

        public OrchestratorGHInputGroup(GH_Group ghGroup) : base(ghGroup)
        {
        }

        public void RequestNewDocumentSolution()
        {
            DocumentSolutionRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
