using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace OrchestratorRhinoCore.Components
{
    public class OrchestratorGHTabular : OrchestratorGHComponent<Dictionary<Guid, Dictionary<string, string>>>
    {
        private GH_Param<GH_String> GHDocumentObjectWithKeys;
        private GH_Param<GH_String> GHDocumentObjectWithValues;
        private IGH_Param GHDocumentObjectWithGeometry;

        public OrchestratorGHTabular(IGH_DocumentObject documentObject) : base(documentObject)
        {
        }

        public OrchestratorGHTabular(IGH_DocumentObject ghDocumentObjectWithKeys,
            IGH_DocumentObject ghDocumentObjectWithValues,
            IGH_DocumentObject ghDocumentObjectWithGeometry) : base(ghDocumentObjectWithGeometry)
        {
            GHDocumentObjectWithKeys = (GH_Param<GH_String>)ghDocumentObjectWithKeys;
            GHDocumentObjectWithValues = (GH_Param<GH_String>)ghDocumentObjectWithValues;
            GHDocumentObjectWithGeometry = (IGH_Param)ghDocumentObjectWithGeometry;
        }

        public void ComputeValue()
        {
            Value = new Dictionary<Guid, Dictionary<string, string>>();


            for (var pathIndex = 0; pathIndex < GHDocumentObjectWithGeometry.VolatileData.Paths.Count; pathIndex++)
            {                
                var id = Guid.NewGuid();

                var obj = GHDocumentObjectWithGeometry.VolatileData.get_Branch(pathIndex)[0];

                if (obj is IGH_GeometricGoo geometricGoo)
                {                   
                    if (geometricGoo.IsReferencedGeometry) id = geometricGoo.ReferenceID;
                }         

                var keys = GHDocumentObjectWithKeys.VolatileData.get_Branch(pathIndex);

                if(keys.Count == 0) continue;

                var values = GHDocumentObjectWithValues.VolatileData.get_Branch(pathIndex);

                var keyValues = new Dictionary<string, string>();

                for (int i = 0; i < keys.Count; i++)
                {
                    keyValues.Add(keys[i].ToString(), values[i].ToString());
                }

                Value.Add(id, keyValues);
            }
        }

        public override void AssignValueToGH()
        {
            
        }
    }
}
