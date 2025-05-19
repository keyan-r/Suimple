using System.Linq;
using Eto.Forms;
using OrchestratorRhinoCore;
using Suimple.Properties;
using Suimple.Utilities;

namespace Suimple.Views.Containers
{
    internal class SuimpleLinearSequencePanel : Scrollable
    {
        internal OrchestratorGHSequence OrchestratorGHSequence;

        internal StackLayout stackLayout;

        internal Button AddNodeButton;

        public SuimpleLinearSequencePanel(OrchestratorGHSequence sequence)
        {
            OrchestratorGHSequence = sequence;

            var autoRunCheckBox = new CheckBox()
            {
                Text = "Auto Update Sliders",
                Checked = Settings.Default.AutoUpdateGH
            };

            autoRunCheckBox.CheckedChanged += (o, e) =>
            {
                Settings.Default.AutoUpdateGH = (bool)autoRunCheckBox.Checked;
            };

            AddNodeButton = new Button()
            {
                Text = "Add Script"
            };

            AddNodeButton.Click += (sender, e) =>
            {
                var newGroupBox = AddNodePanelInGroupBox();
                stackLayout.Items.Insert(stackLayout.Items.Count - 2, 
                    CreateLinkEnableToggle(OrchestratorGHSequence.Links.Last()));
                stackLayout.Items.Insert(stackLayout.Items.Count - 2, 
                    newGroupBox);
            };

            stackLayout = new StackLayout()
            {
                Orientation = Orientation.Vertical,
                Width = 200,
                Padding = 5,
                Spacing = 5,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };

            stackLayout.Items.Add(AddNodePanelInGroupBox());
            stackLayout.Items.Add(AddNodeButton);
            stackLayout.Items.Add(autoRunCheckBox);

            Border = BorderType.None;

            Content = stackLayout;
        }

        private GroupBox AddNodePanelInGroupBox()
        {
            var SuimpleNodePanel = new SuimpleNodePanel();

            if (OrchestratorGHSequence.Nodes.Count > 0)
            {
                var link = new OrchestratorGHNodeLink(OrchestratorGHSequence.Nodes.Last(), SuimpleNodePanel.OrchestratorGhNode);
                OrchestratorGHSequence.Links.Add(link);
            }

            OrchestratorGHSequence.Nodes.Add(SuimpleNodePanel.OrchestratorGhNode);

            var gb = new GroupBox()
            {
                Style = "Style1",
                BackgroundColor = Styling.Medium,
                Padding = new Eto.Drawing.Padding(10),
                Content = SuimpleNodePanel,
            };

            return gb;
        }

        private Panel CreateLinkEnableToggle(OrchestratorGHNodeLink link)
        {
            //var toggleButton = new ToggleButton();
            //toggleButton.BindDataContext(t => t.Checked, (OrchestratorGHNodeLink l) => l.Enabled);
            
            var panel = new Panel();

            var checkBox = new CheckBox();
            checkBox.CheckedBinding.BindDataContext((OrchestratorGHNodeLink l) => link.Enabled);
            checkBox.DataContext = link;
            checkBox.Text = "Link Scripts";

            checkBox.CheckedChanged += (o, e) => link.Enabled = (bool) checkBox.Checked;
            
            var tableLayout = new TableLayout();
            tableLayout.Rows.Add(new TableRow(new TableCell(checkBox, true)));

            panel.Content = tableLayout;

            return panel;
        }
    }
}