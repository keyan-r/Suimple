using System;

using Eto.Forms;

namespace Suimple.Views.Controls
{
    internal class SuimpleFileButton : Button
    {
        internal bool Multiselect = false;
        internal FileFilter Filter { get; set; }

        internal TextBox TextBoxToDisplayResult;

        public SuimpleFileButton(FileFilter fileFilter, TextBox textBoxForResult)
        {
            TextBoxToDisplayResult = textBoxForResult;
            if(fileFilter != null) Filter = fileFilter;

            Image = Utilities.StaticIcons.FindFile;
            Size = new Eto.Drawing.Size(24, 24);

            Click += OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                MultiSelect = this.Multiselect,
            };

            if (Filter != null) dialog.Filters.Add(Filter);

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
                    TextBoxToDisplayResult.Text = dialog.FileName;
                }
            }

        }
    }
}