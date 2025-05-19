using OrchestratorRhinoCore.Components;
using System.Linq;
using Eto.Forms;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Suimple.Views.GeometryPreview;

namespace Suimple.Views.Controls
{
    internal class SuimpleOutputGeometry : SuimpleOutput
    {

        internal GeometryViewer geomViewer = new GeometryViewer();

        public SuimpleOutputGeometry(IOrchestratorGHComponent OrchestratorGHComponent) : base(OrchestratorGHComponent)
        {
            PrimaryControl = geomViewer;
        }

        public override void GetDataFromOrchestrator()
        {

            Application.Instance.Invoke(() =>
            {
                var DocumentObject = OrchestratorGHComponent.DocumentObject;

                if (DocumentObject is GH_PersistentParam<IGH_GeometricGoo> param)
                {
                    var geos = param.VolatileData.AllData(true).Select(x => GH_Convert.ToGeometryBase(x));

                    geomViewer.SetGeometry(geos);
                }

                if (DocumentObject is IGH_Param param2)
                {
                    var geos = param2.VolatileData.AllData(true).Select(x => GH_Convert.ToGeometryBase(x));

                    geomViewer.SetGeometry(geos);
                }

                if (DocumentObject is GH_Component component)
                {
                    if (component.Name != "Context Bake") return;
                    var geos = component.Params.Input[0].VolatileData.AllData(true)
                        .Select(x => GH_Convert.ToGeometryBase(x));

                    geomViewer.SetGeometry(geos);
                }

                if (DocumentObject is IGH_ContextualParameter contextualParameter)
                {

                }
            });
        }
    }
}
