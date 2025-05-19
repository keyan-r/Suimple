using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using OrchestratorCore.Params;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OrchestratorRhinoCore.Components.Inputs;

namespace OrchestratorRhinoCore.Components
{
    public class OrchestratorGHGroup : IOrchestratorParam
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ObservableCollection<IOrchestratorGHComponent> Components = new ObservableCollection<IOrchestratorGHComponent>();

        public OrchestratorGHGroup(GH_Group ghGroup)
        {
            var sortedDocObjects = ghGroup.Objects().OrderBy(o => o.Attributes.Bounds.Y).ThenBy(o => o.Attributes.Bounds.X).ToList();
            
            Name = ghGroup.NickName;

            if (Name.Contains("Attribute Table"))
            {
                SetAttributeTable(sortedDocObjects);
                return;
            }

            if(sortedDocObjects.Count > 0)
            { 
                GatherComponents(sortedDocObjects);
            }

        }


        private void SetAttributeTable(IEnumerable<IGH_DocumentObject> sortedDocObjects)
        {
            GH_Param<GH_String> keyParam = null;
            GH_Param<GH_String> valueParam = null;
            IGH_Param geoParam = null;

            foreach (IGH_DocumentObject documentObject in sortedDocObjects)
            {
                if (documentObject.NickName == "Keys")
                {
                    keyParam = documentObject as GH_Param<GH_String>;
                    continue;
                }

                if (documentObject.NickName == "Values")
                {
                    valueParam = documentObject as GH_Param<GH_String>;
                    continue;
                }

                if (documentObject.NickName.StartsWith("Geo"))
                {
                    geoParam = documentObject as IGH_Param;
                    continue;
                }
            }

            if (keyParam == null || valueParam == null || geoParam == null)
            {
                return;
            }

            var tabular = new OrchestratorGHObjectAttributeFilter(keyParam, valueParam, geoParam);

            Components.Add(tabular);

        }


        public void GatherComponents(IEnumerable<IGH_DocumentObject> documentObjects)
        {
            foreach (var component in documentObjects)
            {
                if (component is IGH_Param ighParam)
                {
                    if (ighParam.Kind != GH_ParamKind.floating)
                    {
                        continue;
                    }

                    //Check if there is an upstream input that is already in the GH Group
                    if (ighParam.Sources.Any(s => documentObjects.Contains(s))) continue;
                }


                if (component is GH_PersistentGeometryParam<IGH_GeometricGoo> geometryParam)
                {
                    Components.Add(new OrchestratorGHGeometry(component));
                    continue;
                }

                switch (component)
                {
                    case GH_NumberSlider slider:
                        Components.Add(new OrchestratorGHPrimitive<double>(slider, DataType.Number));
                        continue;
                    case GH_ValueList valueList:
                        Components.Add(new OrchestratorGHValueList(valueList));
                        continue;
                    case GH_Scribble scribble:
                        Components.Add(new OrchestratorGHLabel(scribble));
                        continue;
                    case GH_Panel panel:
                        if (panel.Recipients.Count == 0)
                        {
                            Components.Add(new OrchestratorGHLabel(panel));
                            continue;
                        }
                        Components.Add(new OrchestratorGHPrimitive<string>(panel, DataType.String));
                        continue;
                    case GH_BooleanToggle toggle:
                        Components.Add(new OrchestratorGHPrimitive<bool>(toggle, DataType.Boolean));
                        continue;
                    //case GH_ButtonObject button:
                    //    Components.Add(new OrchestratorGHPrimitive<bool>(button, DataType.Boolean));
                    //    continue;
                    default: 
                        break;
                }

                switch (component.ComponentGuid.ToString())
                {
                    case "3ede854e-c753-40eb-84cb-b48008f14fd4": //String Param
                    case "fed87bdd-8327-49cd-949c-09d70f3c345c": //Get String Contextual                        
                        var newComponent = new OrchestratorGHPrimitive<string>(component, DataType.String);
                        Components.Add(newComponent);
                        continue;
                    case "06953bda-1d37-4d58-9b38-4b3c74e54c8f": //Get File Path
                    case "e83dd8b2-42aa-41e6-9254-882cddec0a2e": //Get File Path Contextual
                        var isFolder = component.NickName.Contains("Folder") || component.NickName.Contains("folder");
                        Components.Add(new OrchestratorGHPrimitive<string>(component, isFolder ? DataType.Folder: DataType.File));
                        continue;
                    case "3e8ca6be-fda8-4aaf-b5c0-3c54c8bb7312": //Number Param
                    case "7b36b876-9451-46f5-8220-a200d969cc66": //Get Number Contextual
                        Components.Add(new OrchestratorGHPrimitive<double>(component, DataType.Number));
                        continue;
                    case "2e3ab970-8545-46bb-836c-1c11e5610bce": //Integer Param
                    case "b228887e-0852-4d9f-bd46-2591646e0d7c": //Get Integer Contextual
                        Components.Add(new OrchestratorGHPrimitive<int>(component, DataType.Integer));
                        continue;
                    case "cb95db89-6165-43b6-9c41-5702bc5bf137": //Boolean Param
                    case "51ef601d-f86e-4ee4-bcf2-3d459d3e95e9": //Get Boolean Contextual
                        Components.Add(new OrchestratorGHPrimitive<bool>(component, DataType.Boolean));
                        continue;
                    case "ac2bc2cb-70fb-4dd5-9c78-7e1ea97fe278": //Geometry Param
                    case "fbac3e32-f100-4292-8692-77240a42fd1a": //Point Param
                    case "d5967b9f-e8ee-436b-a8ad-29fdcecf32d5": //Curve Param
                    case "deaf8653-5528-4286-807c-3de8b8dad781": //Surface Param
                    case "1e936df3-0eea-4246-8549-514cb8862b7a": //Mesh Param
                    case "919e146f-30ae-4aae-be34-4d72f555e7da": //Brep Param
                        Components.Add(new OrchestratorGHGeometry(component));
                        continue;
                    //case "8529dbdf-9b6f-42e9-8e1f-c7a2bde56a70": //Line Param
                    case "997704ba-c1d3-4262-9090-927c81347ce6": //Get Point

                    default: break;
                }
            }
        }

    }
}
