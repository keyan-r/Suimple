using System;
using Eto.Forms;

namespace Suimple.Utilities
{
    internal static class SuimpleUtilities
    {
        
        public static int GetIntValue(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    return Convert.ToInt32(textBox.Text);
                case DropDown dropDown:
                    return Convert.ToInt32(dropDown.SelectedKey);
                case ToggleButton toggleButton:
                    return Convert.ToInt32(toggleButton.Checked);
                default:
                    return 0;
            }
        }

        public static double GetDoubleValue(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    return Convert.ToDouble(textBox.Text);
                case DropDown dropDown:
                    return Convert.ToDouble(dropDown.SelectedValue);
                case ToggleButton toggleButton:
                    return Convert.ToDouble(toggleButton.Checked);
                default:
                    return 0;
            }
        }

        public static string GetStringValue(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    return textBox.Text;
                case DropDown dropDown:
                    var selectedValue = dropDown.SelectedKey;
                    return selectedValue != null ? selectedValue.ToString() : "";
                case ToggleButton toggleButton:
                    return toggleButton.Checked.ToString();
                default:
                    return "";
            }
        }

        public static bool GetBoolValue(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    return Convert.ToBoolean(textBox.Text);
                case DropDown dropDown:
                    return Convert.ToBoolean(dropDown.SelectedValue);
                case ToggleButton toggleButton:
                    return toggleButton.Checked;
                case CheckBox checkBox:
                    return (bool) checkBox.Checked;
                default:
                    return false;
            }
        }

        public static string OnSelectFolder()
        {
            var dialog = new SelectFolderDialog
            {
                Title = "Select root folder for script library",                
            };
            
            var result = dialog.ShowDialog(Application.Instance.MainForm);
            return result == DialogResult.Ok ? dialog.Directory : "";
        }
    }
}