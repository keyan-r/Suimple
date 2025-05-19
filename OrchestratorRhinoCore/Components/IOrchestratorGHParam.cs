using OrchestratorCore.Params;

namespace OrchestratorRhinoCore.Components
{
    public interface IOrchestratorGHParam : IOrchestratorParam
    {
        void AssignValueToGH();

        void GetValueFromGH();
    }
}