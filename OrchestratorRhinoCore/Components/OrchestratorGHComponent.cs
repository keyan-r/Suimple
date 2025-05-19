using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using OrchestratorCore.Params;

namespace OrchestratorRhinoCore.Components
{
    public abstract class OrchestratorGHComponent<T> : OrchestratorParam<T>, IOrchestratorGHComponent
    {
        public IGH_DocumentObject DocumentObject { get; set; }
        public IGH_DocumentObject CanvasInputComponent { get; }
        public DataType DataType { get; set; }

        protected OrchestratorGHComponent(IGH_DocumentObject documentObject)
        {
            Name = documentObject.NickName;
            DocumentObject = documentObject;

            if (DocumentObject is IGH_Param param && param.SourceCount > 0)
            {
                CanvasInputComponent = param.Sources[0];
            }
            else
            {
                switch (documentObject)
                {
                    case GH_NumberSlider slider:
                        CanvasInputComponent = slider;
                        break;
                    case GH_ValueList valueList:
                        CanvasInputComponent = valueList;
                        break;
                    default: break;
                }
            }
        }
        public abstract void AssignValueToGH();

    }
}