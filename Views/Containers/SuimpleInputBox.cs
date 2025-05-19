using Eto.Drawing;
using Eto.Forms;
using OrchestratorRhinoCore.Components.Inputs;
using OrchestratorRhinoCore.Components;
using Rhino;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Suimple.Properties;
using Suimple.Utilities;
using Suimple.Views.Controls;
using Container = Eto.Forms.Container;

namespace Suimple.Views.Containers
{
    internal class SuimpleInputBox : GroupBox
    {
        public string Name => OrchestratorGHGroup.Name;

        internal OrchestratorGHInputGroup OrchestratorGHGroup;

        internal List<SuimpleInput> formComponents;

        internal Expander expander;

        internal Button triggerButton;

        public SuimpleInputBox(OrchestratorGHInputGroup OrchestratorGhGroup) 
        {
            OrchestratorGHGroup = OrchestratorGhGroup;

            formComponents = new List<SuimpleInput>();

            Style = "Style1";
            
            SetContent();

        }

        private void SetContent()
        {
            expander = new Expander()
            {
                Header = CreateHeader(),
                Expanded = true,
                Width = 200,
                Content = CollectInputComponents()
            };

            foreach (var component in formComponents)
            {
                component.PropertyChanged += (o, e) =>
                {
                    if(Settings.Default.AutoUpdateGH) SolveGroup(component, EventArgs.Empty);
                };
            }

            //BackgroundColor = Colors.LightSteelBlue;
            //BackgroundColor = default;

            BackgroundColor = Styling.ExtraLight;

            Content = expander;
        }

        private Container CreateHeader()
        {
            var headerTable = new TableLayout()
            {
                Padding = 2,
            };

            triggerButton = new Button()
            {
                Text = "Run",
                ToolTip = "Run Grasshopper but only update components in this group"
            };
            triggerButton.MouseDown += (o, e) =>
            {
                BackgroundColor = Colors.CadetBlue;
            };

            triggerButton.Click += SolveGroup;
            triggerButton.MouseUp += (o, e) => BackgroundColor = Styling.ExtraLight;

            headerTable.Rows.Add(new TableRow(new TableCell[]
            {
                new TableCell(triggerButton),
                new TableCell(new Label() { Text = "   " + Name })
            }));

            return headerTable;
        }

        public void PushDataToOrchestrator()
        {
            foreach (var comp in formComponents)
            {
                comp.PushDataToOrchestrator();
            }
        }

        private async void SolveGroup(object sender, EventArgs e)
        {

            if (sender is SuimpleInput input)
            {
                input.PushDataToOrchestrator();
            }
            else
            {
                PushDataToOrchestrator();
            }

            var c = BackgroundColor;
            var c2 = triggerButton.BackgroundColor;

            Application.Instance.Invoke(() =>
            {
                triggerButton.Enabled = false;
                triggerButton.BackgroundColor = Styling.Dark;
                triggerButton.Text = "Running";
                triggerButton.Enabled = false;
                this.BackgroundColor = Colors.Red;
            });

            try
            {
                

              //  await Task.Run(() => OrchestratorGHGroup.RequestNewDocumentSolution());

                //await Task.Run(() =>
                //{
                //    System.Threading.Thread.Sleep(10);
                //});

                OrchestratorGHGroup.RequestNewDocumentSolution();
            }
            finally
            {
                Application.Instance.Invoke(() =>
                {
                    triggerButton.Enabled = true;
                    triggerButton.Text = "Run";
                    triggerButton.Enabled = true;
                    triggerButton.BackgroundColor = c2;
                    BackgroundColor = c;
                });
            }

            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        private Container CollectInputComponents()
        {
            var componentLayout = new DynamicLayout()
            {
                Padding = new Padding(5),
                DefaultSpacing = new Size(10, 10),
            };

            if (OrchestratorGHGroup == null) return null;

            foreach (var comp in OrchestratorGHGroup.Components)
            {
                if (comp is OrchestratorGHGeometry geoComp)
                {
                    formComponents.Add(new SuimpleInputGeometry(geoComp));
                }
                else
                {
                    formComponents.Add(new SuimpleInputData(comp));
                }
            }

            foreach (var comp in formComponents)
            {
                componentLayout.Rows.Add(new DynamicRow(
                    new Label()
                    {
                        Text = comp.Name == "Scribble" ? "" : comp.Name,
                        Width = 100
                    }, 
                    comp.PrimaryControl)
                );

                if (comp.AdditionalFormElements.Count <= 0) continue;

                var additionalControls = new List<Control>() { new Label() };
                additionalControls.AddRange(comp.AdditionalFormElements);
                componentLayout.Rows.Add(new DynamicRow(additionalControls));
            }

            componentLayout.Rows.Add(null);

            return componentLayout;
        }
    }
}
