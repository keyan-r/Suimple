using OrchestratorRhinoCore.Components;

namespace Suimple.Views.Controls
{
    internal abstract class SuimpleOutput : SuimpleFormComponent
    {
        protected SuimpleOutput(IOrchestratorGHComponent OrchestratorGHComponent) : base(OrchestratorGHComponent)
        {
        }

        public abstract void GetDataFromOrchestrator();
    }
}
