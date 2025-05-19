using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using OrchestratorCore;
using OrchestratorRhinoCore.Components;

namespace OrchestratorRhinoCore
{
    public class OrchestratorGHNode : OrchestratorWorkNode, IOrchestratorFileBased
    {
        public GH_Document GH_Document
        {
            get => _ghDocument;
            set
            {
                _ghDocument = value;
                _ghDocument.SolutionEnd -= OnSolutionEnd;
                _ghDocument.SolutionEnd += OnSolutionEnd;
            }
        }

        public event EventHandler FileLoaded;

        public string FilePath { get; set; }

        public ObservableCollection<OrchestratorGHGroup> InputGroups = new ObservableCollection<OrchestratorGHGroup>();
        
        public ObservableCollection<OrchestratorGHGroup> OutputGroups = new ObservableCollection<OrchestratorGHGroup>();

        public ObservableCollection<OrchestratorGHGroup> AllGroups = new ObservableCollection<OrchestratorGHGroup>();

        private GH_Document _ghDocument;

        public OrchestratorGHNode(string filePath)
        {
            FilePath = filePath;
        }

        public string Name => GH_Document?.DisplayName;

        public OrchestratorGHNode()
        {
            var io = Grasshopper.Instances.DocumentServer;
            io.DocumentRemoved += DocumentRemoved;
            this.FileLoaded += (o, e) => { Enabled = true; };
        }
        
        private void OnSolutionEnd(object sender, GH_SolutionEventArgs e)
        {
            OnSolutionEnd();
        }

        private void DocumentRemoved(GH_DocumentServer sender, GH_Document doc)
        {
            if (doc == GH_Document)
            {
                Enabled = false;
            }
        }


        public bool LoadFile()
        {
            //TODO: Check if file exists
            //TODO: Throw error if invalid filepath

            if (string.IsNullOrEmpty(FilePath)) return false;
            GH_Document?.CloseAllSubsidiaries();

            var io = new GH_DocumentIO();
            var success = io.Open(FilePath);

            GH_Document = io.Document;


            RegisterComponents();

            FileLoaded?.Invoke(this, EventArgs.Empty);
            return success;
        }

        public bool LoadFile(string filePath)
        {
            FilePath = filePath;
            return LoadFile();
        }

        public bool LoadFile(GH_Document grasshopperDocument)
        {
            if (grasshopperDocument == null) return false;

            GH_Document = grasshopperDocument;
            FilePath = grasshopperDocument.FilePath;

            RegisterComponents();

            FileLoaded?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void NewSolution(bool expireAllObjects, GH_SolutionMode solutionMode = GH_SolutionMode.Silent)
        {
            foreach (var group in InputGroups)
            {
                foreach (var component in group.Components)
                {
                    component.AssignValueToGH();
                }
            }

            if (GH_Document == null) return;

            GH_Document.Enabled = true;

            //Task.Run(() => GH_Document.NewSolution(expireAllObjects, solutionMode));
            GH_Document.NewSolution(expireAllObjects, solutionMode);
        }

        

        internal void RegisterComponents()
        {
            AllGroups.Clear();
            InputGroups.Clear();
            OutputGroups.Clear();

            if (GH_Document == null) return;

            var groups = new List<GH_Group>();

            foreach(var obj in GH_Document.Objects)
            {
                if (obj is GH_Group ghGroup)
                {
                    groups.Add(ghGroup);
                }
            }

            var sortedGroups = groups.OrderBy(g => g.Attributes.Bounds.X).ThenBy(g => g.Attributes.Bounds.Y);

            foreach (var ghGroup in sortedGroups)
            {               
                if (ghGroup.NickName.StartsWith("Input"))
                {
                    var componentGroup = new OrchestratorGHInputGroup(ghGroup);
                    componentGroup.DocumentSolutionRequested += ComponentGroup_GroupSolveRequested;
                    if (componentGroup.Components.Count <= 0) continue;
                    AllGroups.Add(componentGroup);
                    InputGroups.Add(componentGroup);
                } 
                else if(ghGroup.NickName.StartsWith("Output"))
                {
                    var componentGroup = new OrchestratorGHOutputGroup(ghGroup);
                    //GH_Document.SolutionEnd += componentGroup.UpdateValues;
                    if (componentGroup.Components.Count <= 0) continue;
                    AllGroups.Add(componentGroup);
                    OutputGroups.Add(componentGroup);
                }                
            }
        }

        private async void ComponentGroup_GroupSolveRequested(object sender, EventArgs e)
        {
            var group = (OrchestratorGHInputGroup)sender;

            foreach (var comp in group.Components)
            {
                comp.AssignValueToGH();
            }

            GH_Document.Enabled = true;
            GH_Document.NewSolution(false, GH_SolutionMode.Silent);
        }
    }
}
