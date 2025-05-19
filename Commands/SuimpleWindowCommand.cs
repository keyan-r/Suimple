using Rhino.Commands;
using Rhino.UI;
using Rhino;
using System;
using Suimple.Utilities;
using Suimple.Views.Containers;

namespace Suimple.Commands
{
    public class OpenSuimpleWindowCommand : Command
    {
        /// <summary>
        /// Form accessor
        /// </summary>
        private SuimpleModelessForm Form
        {
            get;
            set;
        }

        public OpenSuimpleWindowCommand()
        {
            Instance = this;
        }

        public static OpenSuimpleWindowCommand Instance
        {
            get;
            private set;
        }

        public override string EnglishName
        {
            get { return "Suimple"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (null == Form)
            {
                Form = new SuimpleModelessForm
                {
                    Owner = RhinoEtoApp.MainWindow,
                    Width = 600,
                    Height = 400,
                    Resizable = true,
                    BackgroundColor = Styling.ExtraLight
                };
                Form.Closed += OnFormClosed;
            }

            Form.Show();
            return Result.Success;
        }

        /// <summary>
        /// FormClosed EventHandler
        /// </summary>
        private void OnFormClosed(object sender, EventArgs e)
        {
            Form.Dispose();
            Form = null;
        }
    }
}
