using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using OrchestratorRhinoCore.Components.Inputs;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;
using Suimple.Views.GeometryPreview;

namespace Suimple.Views.Controls
{
    internal class SuimpleInputGeometry : SuimpleInput
    {
        internal Button getButton = new Button();

        internal List<ObjRef> objectReferences = new List<ObjRef>();

        internal GeometryViewer geomViewer = new GeometryViewer();

        internal OrchestratorGHGeometry OrchestratorGHGeometry => (OrchestratorGHGeometry)OrchestratorGHComponent;

        public SuimpleInputGeometry(OrchestratorGHGeometry OrchestratorGeometryComponent) : base(OrchestratorGeometryComponent)
        {

            string objectType = OrchestratorGeometryComponent.ObjectType.ToString();

            getButton.Text = "Get Objects (" + objectType + ")";
            getButton.Click += (sender, e) => GetObjects("Select " + objectType, OrchestratorGeometryComponent.ObjectType, out objectReferences);
            
            PrimaryControl = getButton;
            AdditionalFormElements.Add(geomViewer);
            //AllControls.Add(getButton);
            //AllControls.Add(geomViewer);
        }


        internal void GetObjects(string prompt, ObjectType objectType, out List<ObjRef> references)
        {
            var getObjects = new GetObject();
            getObjects.SetCommandPrompt(prompt);
            getObjects.GeometryFilter = objectType;
            getObjects.GetMultiple(1, 0);
            if (getObjects.CommandResult() != Result.Success)
            {
                references = new List<ObjRef>();
                return;
            }

            references = getObjects.Objects().ToList();

            geomViewer.SetGeometry(references.Select(o => o.Geometry()).ToList());
        }

        public override void PushDataToOrchestrator()
        {
            OrchestratorGHGeometry.Value = objectReferences;
        }
    }
}
