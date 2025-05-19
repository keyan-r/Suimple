using Grasshopper.Kernel;
using OrchestratorCore.Params;

namespace OrchestratorRhinoCore.Components
{
    public interface IOrchestratorGHComponent : IOrchestratorParam
    {
        IGH_DocumentObject DocumentObject { get; set; }

        IGH_DocumentObject CanvasInputComponent { get; }

        DataType DataType { get; set; }

        void AssignValueToGH();

        //void GetValueFromGH();

    }
}