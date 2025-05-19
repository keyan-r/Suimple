using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Grasshopper.Kernel;
using OrchestratorRhinoCore;
using OrchestratorRhinoCore.Components;
using Rhino;
using Rhino.UI.Controls;
using Suimple.Properties;
using Suimple.Views.Controls;

//using System.Windows.Forms;

namespace Suimple.Views.Containers
{
    internal class SuimpleNodePanel : Expander
    {
        internal OrchestratorGHNode OrchestratorGhNode;

        internal Panel FileManagementPanel;

        internal Panel ContentPanel;

        internal Label NameLabel;
        
        internal List<GroupBox> GroupBoxes = new List<GroupBox>();

        internal ButtonMenuItem SelectFolderMenuItem = new ButtonMenuItem()
        {
            Text = "Choose new Library Location..."
        };

        internal ButtonMenuItem ReloadFolderMenu = new ButtonMenuItem();

        private string lastLoadedLibraryFilepath = "";

        public SuimpleNodePanel()
        {

            NameLabel = new Label()
            {
                Text = "No File Loaded",
                //TextColor = Colors.White,
            };
            NameLabel.Font = new Font(NameLabel.Font.Family, 10, FontStyle.Bold );

            Eto.Style.Add<EtoExpanderHandler>("Style1", h =>
            {
                //h.Control.
            });

            OrchestratorGhNode = new OrchestratorGHNode();
            OrchestratorGhNode.FileLoaded += OrchestratorGhNode_FileLoaded;
            OrchestratorGhNode.SolutionRequested += PushDataToOrchestratorAndTriggerSolution;


            FileManagementPanel = new Panel();
            ContentPanel = new Panel();

            Padding = new Padding(25, 5);

            #region File Selection and Management

            var layout = new DynamicLayout();

            //Dropdown for selecting the script
            var activeFileDropDown = new ActiveFileDropDown();

            //Menu for Library Files
            var libraryButton = new Button()
            {
                Text = "Library",
                Visible = false
            };
            var libraryMenu = InitializeContextMenu();

            var selectFileTextBox = new TextBox()
            {
                Visible = false
            };

            var selectFileButton = new SuimpleFileButton(
                new FileFilter("Grasshopper Files", ".gh"), 
                selectFileTextBox)
            {
                Visible = false
            };

            libraryButton.Click += (sender, e) =>
            {
                libraryMenu.Show(libraryButton);
            };

            //Dropdown for selecting the source of the script
            var loadSourceDropDown = new DropDown()
            {
                Items =
                {
                    "Active Files",
                    "From Library",
                    "From File"
                },
                SelectedIndex = 0
            };

            var showButton = new Button()
            {
                Text = "Show GH",
                Visible = false
            };
            showButton.Click += (o, e) =>
            {
                ShowScriptInGH();
            };

            var loadButton = new Button()
            {
                Text = "Load"
            };

            loadSourceDropDown.SelectedIndexChanged += (sender, e) =>
            {
                switch (loadSourceDropDown.SelectedIndex)
                {
                    case 0:
                        libraryButton.Visible = false;
                        activeFileDropDown.Visible = true;
                        selectFileButton.Visible = false;
                        selectFileTextBox.Visible = false;
                        loadButton.Text = "Load";
                        break;
                    case 1:
                        libraryButton.Visible = true;
                        activeFileDropDown.Visible = false;
                        selectFileButton.Visible = false;
                        selectFileTextBox.Visible = false;
                        showButton.Visible = true;
                        loadButton.Text = "Reload";
                        break;
                    case 2:
                        libraryButton.Visible = false;
                        activeFileDropDown.Visible = false;
                        selectFileButton.Visible = true;
                        selectFileTextBox.Visible = true;
                        showButton.Visible = true;
                        loadButton.Text = "Load";
                        break;
                }
            };

            

            loadButton.Click += (sender, e) =>
            {
                if (GroupBoxes.Count > 0)
                {
                    foreach (var comp in GroupBoxes.OfType<SuimpleOutputBox>().SelectMany(groupBox => (groupBox).formComponents))
                    {
                        if (comp is SuimpleOutputGeometry geoOutput)
                        {
                            geoOutput.geomViewer.PreviewEnabled = false;
                        }
                    }
                }

                switch (loadSourceDropDown.SelectedIndex)
                {
                    case 0:
                        if (activeFileDropDown.ActiveFile == null) return;
                        OrchestratorGhNode.LoadFile(activeFileDropDown.ActiveFile);
                        break;
                    case 1:
                        if (!File.Exists(lastLoadedLibraryFilepath)) return;
                        OrchestratorGhNode.LoadFile(lastLoadedLibraryFilepath);
                        break;
                    case 2:
                        if(selectFileTextBox.Text == "") return;
                        bool exists = System.IO.File.Exists(selectFileTextBox.Text);
                        if(exists) OrchestratorGhNode.LoadFile(selectFileTextBox.Text);
                        break;
                }

                var server = Grasshopper.Instances.DocumentServer;

                
            };

            layout.AddSeparateRow(loadSourceDropDown, activeFileDropDown, libraryButton, selectFileButton, selectFileTextBox, loadButton, showButton, null);

            #endregion

            FileManagementPanel.Content = layout;

            var fullLayout = new DynamicLayout();
            fullLayout.AddSeparateRow(FileManagementPanel);
            fullLayout.AddSeparateRow(ContentPanel);

            fullLayout.Add(null);

            var fullRunButton = new Button()
            {
                Text = "Run Script"
            };
            fullRunButton.Click += PushDataToOrchestratorAndTriggerSolution;

            var closeButton = new Button()
            {
                Text = "x",
                Width = 25,
                Visible = false
            };
            closeButton.Click += (o, e) =>
            {
                OnCloseRequest();
            };

            
            Header = new Panel()
            {
                Content = new TableLayout()
                {
                    Rows =
                    {
                        new TableRow(new TableCell(fullRunButton),
                        new TableCell("  "),
                        new TableCell(NameLabel),
                        new TableCell(""),
                        new TableCell(closeButton),
                        new TableCell("  "))
                    },
                    Spacing = new Size(5, 5),
                    //Padding = new Padding(25, 25)
                }
            };

            Header.MouseEnter += (o, e) => closeButton.Visible = true;
            Header.MouseLeave += (o, e) => closeButton.Visible = false;


            ContentPanel.BindDataContext(l => l.Enabled, (OrchestratorGHNode n) => n.Enabled);
            ContentPanel.DataContext = OrchestratorGhNode;
            
            //BackgroundColor = Styling.ExtraLight;

            Content = fullLayout;

            Expanded = false;
        }

        private void PushDataToOrchestratorAndTriggerSolution(object sender, EventArgs e)
        {
            foreach (var inputGroup in GroupBoxes.OfType<SuimpleInputBox>().ToList())
            {
                inputGroup.PushDataToOrchestrator();
            }

            OrchestratorGhNode.NewSolution(false);
        }

        private void ShowScriptInGH()
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "_Grasshopper");
            var doc = OrchestratorGhNode.GH_Document;

            if (!Grasshopper.Instances.DocumentServer.Contains(doc))
                Grasshopper.Instances.DocumentServer.AddDocument(doc);

            Grasshopper.Instances.ActiveCanvas.Document = doc;

            //Todo: Set the dropdown to Active Files
        }

        private void OrchestratorGhNode_FileLoaded(object sender, EventArgs e)
        {
            NameLabel.Text = OrchestratorGhNode.Name;

            var componentGroups = OrchestratorGhNode.InputGroups.ToList();
            componentGroups.AddRange(OrchestratorGhNode.OutputGroups.ToList());

            foreach (var groupBox in GroupBoxes)
            {
                groupBox.Dispose();
            }
            GroupBoxes.Clear();

            //var sortedGroups = componentGroups.OrderBy(g => g.)

            foreach (var group in OrchestratorGhNode.AllGroups)
            {
                switch (group)
                {
                    case OrchestratorGHInputGroup inputGroup:
                        var inputBox = new SuimpleInputBox(inputGroup);
                        GroupBoxes.Add(inputBox);
                        break;
                    case OrchestratorGHOutputGroup outputGroup:
                        var outputBox = new SuimpleOutputBox(outputGroup);
                        this.OrchestratorGhNode.GH_Document.SolutionEnd -= outputBox.UpdateValues;
                        this.OrchestratorGhNode.GH_Document.SolutionEnd += outputBox.UpdateValues;

                        GroupBoxes.Add(outputBox);
                        break;
                }
            }

            UpdateGroups();
        }

        internal class ActiveFileDropDown : DropDown
        {

            internal GH_Document ActiveFile
            {

                get
                {
                    return SelectedKey == null ? null : activeFiles[ulong.Parse((string)SelectedKey)];
                }
            }

            private Dictionary<ulong, GH_Document> activeFiles = new Dictionary<ulong, GH_Document>();

            internal ActiveFileDropDown()
            {
                var ghServer = Grasshopper.Instances.DocumentServer;
                activeFiles = new Dictionary<ulong, GH_Document>();

                ghServer.DocumentAdded += (sender, e) =>
                {
                    activeFiles.Add(e.RuntimeID, e);

                    Items.Add(new ListItem()
                    {
                        Text = e.DisplayName,
                        Key = e.RuntimeID.ToString()
                    });
                };

                ghServer.DocumentRemoved += (sender, e) =>
                {
                    Items.Remove(Items.FirstOrDefault(item => item.Key == e.RuntimeID.ToString()));
                    activeFiles.Remove(e.RuntimeID);
                };

                foreach (GH_Document doc in ghServer)
                {
                    activeFiles.Add(doc.RuntimeID, doc);

                    var li = new ListItem()
                    {
                        Text = doc.DisplayName,
                        Key = doc.RuntimeID.ToString()
                    };

                    Items.Add(li);
                }

                SelectedIndex = 0;
            }
        }

        private void UpdateGroups()
        {
            var groupLayout = new DynamicLayout();
            foreach (var groupBox in GroupBoxes)
            {
                if (groupBox is SuimpleOutputBox outputBox)
                {
                    outputBox.UpdateValues(null, null);
                }
                groupLayout.Rows.Add(new DynamicRow(groupBox));
            }
            if(GroupBoxes.Count == 0)
            {
                groupLayout.Rows.Add(new DynamicRow(new Label() { Text = "No designated Inputs or Outputs" }));
            }
            groupLayout.Rows.Add(null);
            ContentPanel.Content = groupLayout;
        }

        private ContextMenu InitializeContextMenu()
        {
            var contextMenu = new ContextMenu();

            SelectFolderMenuItem.Click += (sender, e) =>
            {
                var newPath = Utilities.SuimpleUtilities.OnSelectFolder();

                if (!Directory.Exists(newPath)) return;
                //Todo : throw error for incorrect folder path

                Settings.Default.LibraryRootPath = newPath;
                Settings.Default.Save();

                ResetLibraryMenu(contextMenu, newPath);
            };            

            string rootPath = Settings.Default.LibraryRootPath;

            if(!string.IsNullOrEmpty(rootPath))
            {
                ResetLibraryMenu(contextMenu, rootPath);                
            }
            
            contextMenu.Items.Add(new SeparatorMenuItem());

            contextMenu.Items.Add(SelectFolderMenuItem);            

            return contextMenu;
        }

        private void ResetLibraryMenu(ContextMenu contextMenu, string rootPath)
        {
            if(!Directory.Exists(rootPath))
            {
                return;                
            }

            //Get the filepaths for all files ending in .gh
            var files = new string[0];
            try
            {
                files = Directory.GetFiles(rootPath, "*.gh", SearchOption.AllDirectories);
            }
            catch
            {
                //TODO: Throw error if a folder cannot be accessed
            }

            var menuDictionary = new Dictionary<string, SubMenuItem>();

            contextMenu.Items.Clear();

            foreach(var file in files)
            {
                var directory = Path.GetDirectoryName(file);

                directory.Replace(rootPath, "");

                var menuItem = GetMenuItem(directory);

                var scriptItem = new ButtonMenuItem()
                {
                    Text = Path.GetFileName(file),
                    Tag = file
                };

                scriptItem.Click += (sender, e) =>
                {
                    if(OrchestratorGhNode.LoadFile(file)) lastLoadedLibraryFilepath = file;
                };

                menuItem.Items.Add(scriptItem);                
                continue;

                SubMenuItem GetMenuItem(string s)
                {
                    if(menuDictionary.TryGetValue(s, out var item))
                    {
                        return item;
                    }

                    var newMenuItem = new SubMenuItem()
                    {
                        Text = Path.GetFileName(s)
                    };

                    menuDictionary.Add(s, newMenuItem);

                    if (s == rootPath)
                    {
                        contextMenu.Items.Add(newMenuItem); 
                        return newMenuItem;
                    }

                    var parent = Path.GetDirectoryName(s);
                    var parentItem = GetMenuItem(parent);

                    parentItem.Items.Add(newMenuItem);

                    return newMenuItem;
                }
            }
        }

        internal void OnCloseRequest()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        internal event EventHandler CloseRequested;
    }
}
