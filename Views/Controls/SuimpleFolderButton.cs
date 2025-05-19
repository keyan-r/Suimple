using Eto.Forms;
using System;

namespace Suimple.Views.Controls
{
    internal class SuimpleFolderButton : Button
    {
        internal bool Multiselect = false;
        internal FileFilter Filter { get; set; }

        internal TextBox TextBoxToDisplayResult;

        public SuimpleFolderButton(TextBox textBoxForResult)
        {
            TextBoxToDisplayResult = textBoxForResult;

            Image = Utilities.StaticIcons.FindFolder;
            Size = new Eto.Drawing.Size(24, 24);

            Click += OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            var dialog = new SelectFolderDialog();

            var result = DialogResult.None;
            try
            {
                result = dialog.ShowDialog(this.ParentWindow);
            }
            catch
            {

            }
            finally
            {
                if (result == DialogResult.Ok)
                {
                    TextBoxToDisplayResult.Text = dialog.Directory;
                }
            }
        }
    }
}