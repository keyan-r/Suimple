using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using Grasshopper.Kernel;
using OrchestratorRhinoCore.Components.Inputs;
using OrchestratorRhinoCore.Components;
using Suimple.Utilities;
using Suimple.Views.Controls;

namespace Suimple.Views.Containers
{
    internal class SuimpleOutputBox : GroupBox
    {
        public String Name => OrchestratorGHGroup.Name;

        internal OrchestratorGHOutputGroup OrchestratorGHGroup;

        internal List<SuimpleOutput> formComponents;

        internal Expander expander;

        public SuimpleOutputBox(OrchestratorGHOutputGroup OrchestratorGhGroup)
        {
            OrchestratorGHGroup = OrchestratorGhGroup;

            formComponents = new List<SuimpleOutput>();

            SetContent();

            UpdateValues(null, null);

            Style = "Style1";
            TextColor = Colors.White;
            BackgroundColor = Styling.Light;
        }

        private void SetContent()
        {
            expander = new Expander()
            {
                Header = CreateHeader(),
                Expanded = true,
                Content = CollectOutputComponents()
            };

            Content = expander;
        }

        private Container CreateHeader()
        {
            var headerTable = new TableLayout()
            {
                Padding = 2,
            };

            headerTable.Rows.Add(new TableRow(new TableCell[]
            {
                new TableCell(new Label() { Text = "   " + Name })
            }));

            return headerTable;
        }

        private Container CollectOutputComponents()
        {
            var componentLayout = new DynamicLayout()
            {
                Padding = new Padding(5),
                DefaultSpacing = new Size(10, 10),
            };

            if (OrchestratorGHGroup == null) return null;

            formComponents.Clear();

            foreach (var comp in OrchestratorGHGroup.Components)
            {
                if (comp is OrchestratorGHGeometry geoComp)
                {
                    formComponents.Add(new SuimpleOutputGeometry(geoComp));
                }

                //if (comp is OrchestratorGHObjectAttributeFilter tabComp)
                //{
                //    formComponents.Add(new SuimpleOutputFilterTree(tabComp));
                //}
                else
                {
                    formComponents.Add(new SuimpleOutputData(comp));
                }
            }

            foreach (var comp in formComponents)
            {
                componentLayout.Rows.Add(new DynamicRow(new Label() { Text = comp.Name }, comp.PrimaryControl));
                if (comp.AdditionalFormElements.Count > 0)
                {
                    var additionalControls = new List<Control>() { new Label() };
                    additionalControls.AddRange(comp.AdditionalFormElements);
                    componentLayout.Rows.Add(new DynamicRow(additionalControls));
                }
            }

            componentLayout.Rows.Add(null);

            return componentLayout;
        }

        public void UpdateValues(object sender, GH_SolutionEventArgs solutionEventArgs)
        {
            //if (solutionEventArgs == null) return;

            var solutionState = GH_ProcessStep.PostProcess;

            if (solutionEventArgs != null)
            {
                solutionState = solutionEventArgs.Document.SolutionState;
            }

            if(solutionState != GH_ProcessStep.PostProcess) return;

            foreach (var comp in formComponents)
            {
                comp.GetDataFromOrchestrator();
            }
        }
    }
}
