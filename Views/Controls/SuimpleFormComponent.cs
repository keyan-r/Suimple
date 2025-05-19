using System.Collections.Generic;
using Eto.Forms;
using OrchestratorRhinoCore.Components;

namespace Suimple.Views.Controls
{
    internal abstract class SuimpleFormComponent
    {
        internal string Name => OrchestratorGHComponent.Name;
        internal string Description => OrchestratorGHComponent.Description;

        internal Control PrimaryControl;

        internal List<Control> AllControls
        {
            get
            {
                var controls = new List<Control>();
                controls.Add(new Label() {Text = Name, Wrap = WrapMode.Word, Width = 100});
                controls.Add(PrimaryControl);
                controls.AddRange(AdditionalFormElements);

                return controls;
            }
        }

        internal List<Control> AdditionalFormElements = new List<Control>();

        internal IOrchestratorGHComponent OrchestratorGHComponent;

        internal SuimpleFormComponent(IOrchestratorGHComponent OrchestratorGHComponent)
        {
            
            this.OrchestratorGHComponent = OrchestratorGHComponent;
        }


    }
}
