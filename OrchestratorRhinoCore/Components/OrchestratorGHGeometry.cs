using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.DocObjects;

namespace OrchestratorRhinoCore.Components.Inputs
{
    public class OrchestratorGHGeometry : OrchestratorGHComponent<List<ObjRef>>
    {
        public ObjectType ObjectType = ObjectType.AnyObject;

        public OrchestratorGHGeometry(IGH_DocumentObject documentObject) : base(documentObject)
        {
            DataType = OrchestratorCore.Params.DataType.Geometry;

            string geometryType = "";
            if (documentObject is IGH_Param param)
            {
                geometryType = param.TypeName;
            }            
            Enum.TryParse(geometryType, true, out ObjectType);
        }


        public override void AssignValueToGH()
        {
            if(DocumentObject is IGH_Param ighParam)
            {
                if (ighParam.Kind != GH_ParamKind.floating) return;

                ighParam.ExpireSolution(false);
                ighParam.VolatileData.Clear();


                var refGeo = Value.Select(GH_Convert.ObjRefToGeometry);

                ighParam.AddVolatileDataList(new GH_Path(0), refGeo);
            }
        }

        //public override void GetValueFromGH()
        //{
        //    //throw new NotImplementedException();
        //}
    }
}