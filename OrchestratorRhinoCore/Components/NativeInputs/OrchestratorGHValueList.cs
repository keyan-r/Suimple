using Grasshopper.Kernel.Special;
using OrchestratorCore.Params;
using OrchestratorRhinoCore.Components.Inputs;

namespace OrchestratorRhinoCore.Components
{
    public class OrchestratorGHValueList : OrchestratorGHPrimitive<string>
    {
        public OrchestratorGHValueList(GH_ValueList valueList, DataType dataType = DataType.String) : base(valueList, dataType)
        {
        }

        public override void AssignValueToGH()
        {
            var valueList = (GH_ValueList)DocumentObject;

            switch (valueList.ListMode)
            {
                case GH_ValueListMode.DropDown:
                    valueList.SelectItem(valueList.ListItems.FindIndex(li => li.Expression.Trim('"') == Value));
                    break;
                case GH_ValueListMode.Sequence:
                    break;
                case GH_ValueListMode.Cycle:
                    break;
                case GH_ValueListMode.CheckList:
                    break;
                default:
                    break;
            }
        }
    }
}
