using Eto.Forms;
using Grasshopper.Kernel;
using OrchestratorRhinoCore.Components;
using System.Linq;

namespace Suimple.Views.Controls
{
    internal class SuimpleOutputData : SuimpleOutput
    {       

        internal string Value = "";

        internal ListBox ResultsListBox = new ListBox();

        internal TextBox TextBox = new TextBox();

        internal Panel ResultsPanel = new Panel();

        public SuimpleOutputData(IOrchestratorGHComponent OrchestratorGHComponent) : base(OrchestratorGHComponent)
        {
            ResultsPanel.Content = TextBox;
            this.OrchestratorGHComponent = OrchestratorGHComponent;
            PrimaryControl = ResultsPanel;
        }

        public override void GetDataFromOrchestrator()
        {
            Application.Instance.Invoke(() =>
            {
                var ghComponent = OrchestratorGHComponent.DocumentObject as GH_Component;

                IGH_Param param;
                if (ghComponent != null) { param = ghComponent.Params.Input[0]; }

                if (OrchestratorGHComponent.DocumentObject is IGH_Param)
                {
                    param = OrchestratorGHComponent.DocumentObject as IGH_Param;
                }
                else
                {
                    return;
                }

                var strings = param.VolatileData.AllData(true).Select(x => x.ToString()).ToList();

                if (strings.Count == 1)
                {
                    TextBox.Text = strings[0];
                    ResultsPanel.Content = TextBox;
                    return;
                }

                ResultsListBox.Items.Clear();
                //ResultsListBox.Items.AddRange(strings.Select(s => new ListItem() {Text = s}));

                foreach (var s in strings)
                {
                    ResultsListBox.Items.Add(s);
                }

                ResultsPanel.Content = ResultsListBox;
            });
            
        }
    }
}
