using Eto.Forms;
using Rhino.Geometry;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Grasshopper;
using Rhino.UI.Controls;
using Application = Eto.Forms.Application;
using Button = Eto.Forms.Button;
using CheckBox = Eto.Forms.CheckBox;
using Label = Eto.Forms.Label;
using Padding = Eto.Drawing.Padding;
using Panel = Eto.Forms.Panel;

namespace Suimple.Views.GeometryPreview
{
    internal class GeometryViewer : Panel
    {
        internal PreviewConduit Conduit;

        internal BoundingBox ZoomBox = BoundingBox.Empty;

        internal bool PreviewEnabled
        {
            get => Conduit.Enabled;
            set
            {
                Conduit.Enabled = value;
                RhinoDoc.ActiveDoc.Views.Redraw();
                viewportControl.Refresh();
            }
        }

        internal ColorPicker colorPicker = new ColorPicker()
        {
            Value = Color.Parse("Aqua")
        };

        internal CheckBox rhinoPreviewCheckBox = new CheckBox()
        {
            Text = "Rhino"
        };

        internal CheckBox OrchestratorPreviewCheckBox = new CheckBox()
        {
            Text = "Viewport"
        };

        internal Button zoomButton = new Button()
        {
            Text = "Zoom"
        };

        internal DynamicLayout dynamicLayout;

        internal ViewportControl viewportControl;

        internal Label HeaderLabel;
        internal GeometryViewer()
        {
            colorPicker.ValueChanged += UpdateColor;
            rhinoPreviewCheckBox.CheckedChanged += OnToggle;
            OrchestratorPreviewCheckBox.CheckedChanged += OnToggle;

            viewportControl = new ViewportControl();
            viewportControl.Viewport.ConstructionGridVisible = false;
            viewportControl.Viewport.ConstructionAxesVisible = false;
            viewportControl.Viewport.WorldAxesVisible = false;
            viewportControl.Size = new Size(400, 200);

            Conduit = new PreviewConduit(viewportControl);

            zoomButton.Click += (o, e) =>
            {
                ZoomViewport();
            };

            HeaderLabel = new Label() {Text = "0 objects"};
            var previewLabel = new Label() { Text = "Preview" };
            previewLabel.Font = new Font(previewLabel.Font.Family, 12);

            var expander = new Expander()
            {
                Header = HeaderLabel,
                Expanded = false
            };

            dynamicLayout = new DynamicLayout()
            {
                Padding = new Padding(4),
                Spacing = new Size(10, 8),
                DefaultSpacing = new Size(12, 8)
            };
            dynamicLayout.BeginVertical();
            dynamicLayout.EndBeginVertical();
            dynamicLayout.AddSeparateRow(previewLabel);
            dynamicLayout.AddSeparateRow(rhinoPreviewCheckBox, OrchestratorPreviewCheckBox, zoomButton, colorPicker);
            dynamicLayout.AddSeparateRow(viewportControl);
            dynamicLayout.AddRow(null);
            dynamicLayout.EndVertical();

            expander.Content = dynamicLayout;

            this.Content = expander;
        }


        internal void UpdateColor(object sender, EventArgs e)
        {
            if (sender is ColorPicker cp)
            {
                var c = cp.Value;
                c.A = (float)0.5;
                c.Ab = 150;
                Conduit.previewColor = c.ToSD();

            }

            if (Conduit.Enabled)
            {
                viewportControl.Refresh();
            }

            if ((bool)rhinoPreviewCheckBox.Checked)
            {
                RhinoDoc.ActiveDoc.Views.Redraw();
            }
        }

        internal void OnToggle(object sender, EventArgs e)
        {
            Conduit.DrawInControl = (bool)OrchestratorPreviewCheckBox.Checked;
            Conduit.DrawInRhino = (bool)rhinoPreviewCheckBox.Checked;

            Conduit.Enabled = Conduit.DrawInControl || Conduit.DrawInRhino;

            RhinoDoc.ActiveDoc.Views.Redraw();
            viewportControl.Refresh();

        }

        internal void SetGeometry(IEnumerable<GeometryBase> geometries)
        {
            HeaderLabel.Text = geometries.Count().ToString() + " object(s)";

            Conduit.LoadGeometry(geometries);
            ZoomBox = BoundingBox.Empty;

            foreach (GeometryBase geom in geometries)
            {
                ZoomBox.Union(geom.GetBoundingBox(false));
            }

            if ((bool)OrchestratorPreviewCheckBox.Checked && ZoomBox.IsValid)
            {
                Application.Instance.Invoke(() =>
                {
                    viewportControl.Viewport.ZoomBoundingBox(ZoomBox);
                    viewportControl.Refresh();
                });
            }

            if ((bool)rhinoPreviewCheckBox.Checked)
            {
                var vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
            }
        }

        private void ZoomViewport()
        {
            
            if ((bool)OrchestratorPreviewCheckBox.Checked && ZoomBox.IsValid)
            {
                Application.Instance.Invoke(() =>
                {
                    viewportControl.Viewport.ZoomBoundingBox(ZoomBox);
                    //viewportControl.Viewport.SetClippingPlanes(ZoomBox);
                    viewportControl.Refresh();
                });
            }

            if ((bool)rhinoPreviewCheckBox.Checked && ZoomBox.IsValid)
            {
                var vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
                vp.ZoomBoundingBox(ZoomBox);
            }
        }
    }
}
