using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using OrchestratorCore.Params;
using OrchestratorRhinoCore.Components.Inputs;
using OrchestratorRhinoCore.Components;
using Eto.Drawing;
using Grasshopper.GUI.Base;
using Rhino;
using Suimple.Utilities;

namespace Suimple.Views.Controls
{
    internal class SuimpleInputData : SuimpleInput
    {       
        public SuimpleInputData(IOrchestratorGHComponent OrchestratorGHComponent) : base(OrchestratorGHComponent)
        {
            this.OrchestratorGHComponent = OrchestratorGHComponent;

            SetFormControl();
        }

        public override void PushDataToOrchestrator()
        {
            try
            {
                switch (OrchestratorGHComponent)
                {
                    case OrchestratorGHPrimitive<int> intComponent:
                        intComponent.Value = SuimpleUtilities.GetIntValue(PrimaryControl);
                        break;
                    case OrchestratorGHPrimitive<double> doubleComponent:
                        doubleComponent.Value = SuimpleUtilities.GetDoubleValue(PrimaryControl);
                        break;
                    case OrchestratorGHPrimitive<string> stringComponent:
                        stringComponent.Value = SuimpleUtilities.GetStringValue(PrimaryControl);
                        break;
                    case OrchestratorGHPrimitive<bool> boolComponent:
                        boolComponent.Value = SuimpleUtilities.GetBoolValue(PrimaryControl);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                //TODO: Implement popup notification
                RhinoApp.WriteLine(e.Message);
            }
        }

        private void SetFormControl()
        {
            if (OrchestratorGHComponent is OrchestratorGHLabel OrchestratorGhLabel)
            {
                switch (OrchestratorGhLabel.DocumentObject)
                {
                    case GH_Panel panel:
                        PrimaryControl = new Label()
                        {
                            Text = panel.UserText,
                            Wrap = WrapMode.Word
                        };
                        break;
                    case GH_Scribble scribble:
                        var label = new Label()
                        {
                            Text = scribble.Text,
                            Wrap = WrapMode.Word
                        };
                        var font = label.Font;
                        label.Font = new Font(scribble.Font.FontFamily.Name, (float)(scribble.Font.Size * 0.5));
                        PrimaryControl = label;
                        break;
                }

                return;
            }

            if (null == OrchestratorGHComponent.CanvasInputComponent)
            {
                if (OrchestratorGHComponent is OrchestratorGHPrimitive<bool>)
                {
                    if (OrchestratorGHComponent.DocumentObject is GH_ButtonObject button)
                    {
                        
                    }
                    var control = new CheckBox();
                    if (OrchestratorGHComponent.DocumentObject is GH_BooleanToggle toggle)
                    {
                        control.Checked = toggle.Value;
                    }
                    control.CheckedChanged += (o,e) => OnPropertyChanged();
                    PrimaryControl = control;
                    return;
                }

                var textBox = new TextBox();
                switch (OrchestratorGHComponent.DataType)
                {
                    case DataType.File:
                    {
                        AdditionalFormElements.Add(new SuimpleFileButton(null, textBox));
                        break;
                    }
                    case DataType.Folder:
                    {
                        AdditionalFormElements.Add(new SuimpleFolderButton(textBox));
                        break;
                    }
                }

                PrimaryControl = textBox;
                return;
            }

            switch (OrchestratorGHComponent.CanvasInputComponent)
            {
                case GH_NumberSlider numberSlider:
                    PrimaryControl = ConvertSlider(numberSlider);
                    break;
                case GH_BooleanToggle booleanToggle:
                    var toggleControl = new CheckBox();
                    toggleControl.Checked = booleanToggle.Value;
                    toggleControl.CheckedChanged += (o, e) => OnPropertyChanged();
                    PrimaryControl = toggleControl;
                    break;
                case GH_ButtonObject buttonObject:
                    var control = new CheckBox();
                    control.CheckedChanged += (o, e) => OnPropertyChanged();
                    PrimaryControl = control;
                    break;
                case GH_ValueList valueList:
                    PrimaryControl = ConvertDropDown(valueList);
                    break;
                case GH_Panel panel:
                    PrimaryControl = new TextBox() { Text = panel.UserText};
                    break;
                case IGH_ContextualParameter contextualParameter:
                    if (OrchestratorGHComponent.CanvasInputComponent.Name == "Get Boolean")
                    {
                        PrimaryControl = ConvertButton(OrchestratorGHComponent.CanvasInputComponent);
                    }
                    else
                    {
                        PrimaryControl = new TextBox();
                    }
                    break;
                default:
                    PrimaryControl = new TextBox();
                    break;
            }

            //AllControls.Add(PrimaryControl);
        }

        #region Control Conversions
        private Control ConvertButton(IGH_DocumentObject buttonParam)
        {

            bool isSet = false;

            if (buttonParam is GH_BooleanToggle booleanToggle)
            {
                isSet = booleanToggle.Value;
            }
            var button = new ToggleButton()
            {
                Text = isSet.ToString(),
                Checked = isSet
            };

            button.CheckedChanged += (object sender, EventArgs args) =>
            {
                button.Text = button.Checked ? "True" : "False";
            };


            return button;
        }

        internal Control ConvertDropDown(GH_ValueList ghValueList)
        {
            var listItems = new List<ListItem>();
            var listStrings = new List<string>();

            if (ghValueList.NickName.StartsWith("Layer"))
            {
                listStrings = RhinoDoc.ActiveDoc.Layers.Where(layer => !layer.IsDeleted).Select(l => l.FullPath).ToList();
                listItems.AddRange(listStrings.Select(listString => new ListItem() { Text = listString}));
            }
            else if (ghValueList.NickName.StartsWith("Block"))
            {
                listStrings = RhinoDoc.ActiveDoc.InstanceDefinitions.Select(b => b.Name).ToList();
                listItems.AddRange(listStrings.Select(listString => new ListItem() { Text = listString}));
            }
            else
            {
                listItems.AddRange(ghValueList.ListItems.Select(listItem => new ListItem() { Text = listItem.Name, Key = (listItem.Expression).Trim('"') }));
            }

            //Add options for checkboxes
            var dropDown = new DropDown();
            dropDown.Items.AddRange(listItems);
            dropDown.SelectedIndex = ghValueList.ListItems.IndexOf(ghValueList.FirstSelectedItem);

            dropDown.SelectedIndexChanged += (o, e) => OnPropertyChanged();

            return dropDown;
        }
        
        internal Control ConvertSlider(GH_NumberSlider ghNumberSlider)
        {
            double ghSliderMax = (double)ghNumberSlider.Slider.Maximum;
            double ghSliderMin = (double)ghNumberSlider.Slider.Minimum;
            var ghSliderTickFrequency = ghNumberSlider.Slider.TickFrequency;
            var ghSliderValue = (double) ghNumberSlider.Slider.Value;

            var isInt = ghNumberSlider.Slider.Type == GH_SliderAccuracy.Integer;

            var slider = new CustomSlider(ghSliderMin, ghSliderMax, isInt ? 0 : ghNumberSlider.Slider.DecimalPlaces)
            {
              Value = (int)( ghSliderValue / (ghSliderMax - ghSliderMin) * 100),
              TickFrequency = ghSliderTickFrequency,
            };

            AdditionalFormElements.Add(slider);
            var sliderTextBox = new TextBox()
            {

                Text = ghSliderValue.ToString(),

            };

            slider.ValueChanged += OnSliderValueChanged;

            return sliderTextBox;
        }

        internal void OnSliderValueChanged(object sender, EventArgs e)
        {
            var slider = (CustomSlider)sender;
            var sliderTextBox = (TextBox)PrimaryControl;
            sliderTextBox.Text = Math.Round( slider.MappedValue, slider.Decimals).ToString();

            OnPropertyChanged();
        }

        internal class CustomSlider : Slider
        {
            private double _referenceMin = 0;
            private double _referenceMax = 100;
            internal int Decimals = 2;

            public CustomSlider(double referenceMin, double referenceMax, int decimals)
            {
                _referenceMin = referenceMin;
                _referenceMax = referenceMax;
                Decimals = decimals;
            }

            internal double MappedValue
            {
                get
                {
                    var sliderRange = _referenceMax - _referenceMin;
                    var sliderValueMapped = Value * sliderRange / 100 + _referenceMin;
                    return sliderValueMapped;
                }
            }
        }

        #endregion
    }
}
