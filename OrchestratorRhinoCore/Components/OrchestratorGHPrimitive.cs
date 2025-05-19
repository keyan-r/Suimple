using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using OrchestratorCore.Params;

namespace OrchestratorRhinoCore.Components.Inputs
{
    public class OrchestratorGHPrimitive<T> : OrchestratorGHComponent<T>
    {

        public OrchestratorGHPrimitive(IGH_DocumentObject component, DataType dataType) : base(component)
        {
            DocumentObject = component;
            DataType = dataType;
        }

        public override void AssignValueToGH()
        {
            if (this.Value is IEnumerable<T>) return;

            switch (DocumentObject)
            {
                case GH_NumberSlider slider when Value is double value:
                    //slider.TrySetSliderValue((decimal)value);
                    slider.SetSliderValue((decimal)value);
                    OnPropertyChanged(); return;
                case GH_Panel panel when Value is string panelValue:
                    panel.SetUserText(panelValue);
                    OnPropertyChanged(); return;
                case GH_ButtonObject button when Value is bool buttonBool:
                    if (buttonBool) button.ButtonDown = true;
                    OnPropertyChanged();
                    return;
                case GH_BooleanToggle toggle when Value is bool value:
                    toggle.Value = value;
                    toggle.ExpireSolution(false);
                    //toggle.SetPrincipal(value, true, true);
                    OnPropertyChanged(); return;
                default:
                    break;
            }

            IGH_Goo ghValue = null;


            switch (Value)
            {
                case bool boolValue:
                    ghValue = new GH_Boolean(boolValue);
                    break;
                case int intValue:
                    ghValue = new GH_Integer(intValue);
                    break;
                case double doubleValue:
                    ghValue = new GH_Number(doubleValue);
                    break;
                case string stringValue:
                    ghValue = new GH_String(stringValue);
                    break;
                default:
                    break;
            }

            //ghValue = new GH_String(StringValue ?? "");

            var ghNumber = new GH_Number();
            var ghString = new GH_String();
            var ghBool = new GH_Boolean();
            var ghInteger = new GH_Integer();

            GH_Convert.ToGHString(ghValue, GH_Conversion.Both, ref ghString);
            GH_Convert.ToGHNumber(ghValue, GH_Conversion.Both, ref ghNumber);
            GH_Convert.ToGHBoolean(ghValue, GH_Conversion.Both, ref ghBool);
            GH_Convert.ToGHInteger(ghValue, GH_Conversion.Both, ref ghInteger);

            if (DocumentObject is IGH_ContextualParameter contextualParameter)
            {
                var type = contextualParameter.GetType().ToString();                

                switch (type)
                {
                    case "ContextualComponents.GetStringParameter":
                    case "ContextualComponents.GetFilePathParameter":
                        contextualParameter.AssignContextualData(new[] { ghString });
                        break;
                    case "ContextualComponents.GetNumberParameter":
                        contextualParameter.AssignContextualData(new[] { ghNumber });
                        break;
                    case "ContextualComponents.GetBooleanParameter":
                        contextualParameter.AssignContextualData(new[] { ghBool });
                        break;
                    case "ContextualComponents.GetIntegerParameter":
                        contextualParameter.AssignContextualData(new[] { ghInteger });
                        break;
                    default:
                        break;
                }
                OnPropertyChanged();
                return;
            }

            switch (DocumentObject)
            {
                case GH_PersistentParam<GH_Boolean> boolParam:
                    boolParam.ExpireSolution(true);
                    boolParam.PersistentData.ClearData();
                    boolParam.SetPersistentData(ghBool);
                    boolParam.VolatileData.Clear();
                    boolParam.AddVolatileData(new GH_Path(0), 0, ghBool);
                    break;
                case GH_PersistentParam<GH_String> stringParam:
                    stringParam.ExpireSolution(true);
                    stringParam.PersistentData.ClearData();
                    stringParam.SetPersistentData(ghString);
                    stringParam.VolatileData.Clear();
                    stringParam.AddVolatileData(new GH_Path(0), 0, ghString);
                    
                    break;
                case GH_PersistentParam<GH_Number> numberParam:
                    numberParam.ExpireSolution(true);
                    numberParam.VolatileData.Clear();
                    numberParam.PersistentData.ClearData();
                    numberParam.SetPersistentData(ghNumber);
                    numberParam.AddVolatileData(new GH_Path(0), 0, ghNumber);
                    break;
                case GH_PersistentParam<GH_Integer> integerParam:
                    integerParam.ExpireSolution(true);
                    integerParam.VolatileData.Clear();
                    integerParam.PersistentData.ClearData();
                    integerParam.SetPersistentData(ghInteger);
                    integerParam.AddVolatileData(new GH_Path(0), 0, ghInteger);
                    break;

                default: break;
            }
            OnPropertyChanged();
        }
    }

}