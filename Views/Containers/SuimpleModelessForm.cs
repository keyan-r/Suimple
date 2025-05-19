using Eto.Drawing;
using Eto.Forms;
using System;
using System.ComponentModel;
using Rhino.UI;
using Suimple.Utilities;

namespace Suimple.Views.Containers
{
    class SuimpleModelessForm : Form
    {
        public SuimpleModelessForm()
        {
            Maximizable = true;
            Minimizable = true;
            Padding = new Padding(5);
            Resizable = true;
            ShowInTaskbar = false;
            Title = "Suimple";
            WindowStyle = WindowStyle.Default;
            BackgroundColor = Colors.White;

            var scrollable = new Scrollable()
            {
                Padding = new Padding(20),
                Border = BorderType.None,
                BackgroundColor = Colors.White
            };

            scrollable.Content = SuimplePlugin.Instance.sequencePanels[0];
            scrollable.BackgroundColor = Styling.ExtraLight;

            Content = scrollable;

        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            this.RestorePosition();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SavePosition();
            base.OnClosing(e);
        }

    }
}
