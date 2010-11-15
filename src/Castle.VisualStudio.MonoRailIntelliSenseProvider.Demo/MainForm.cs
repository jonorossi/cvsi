// Copyright 2007-2008 Jonathon Rossi - http://www.jonorossi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.VisualStudio.MonoRailIntelliSenseProvider.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Castle.NVelocity.Ast;

    public enum TreeViewImages
    {
        Assembly, Class, Error, Field, Information, Macro, Method, Namespace, Property, Variable, Warning
    }

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            typesTreeView.ImageList = treeViewImageList;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            
            typesTreeView.BeginUpdate();
            typesTreeView.Nodes.Clear();

            DateTime startTime = DateTime.Now;

            IntelliSenseProvider intelliSenseProvider = new IntelliSenseProvider(binDirectoryTextBox.Text);

            // Add Helpers
            TreeNode helperTreeNode = new TreeNode("Helpers");
            SetImageKey(helperTreeNode, TreeViewImages.Information);
            typesTreeView.Nodes.Add(helperTreeNode);
            AddClassNodesToTreeView(intelliSenseProvider.GetHelpers(), helperTreeNode);

            // Add View Components
            TreeNode viewComponentTreeNode = new TreeNode("View Components");
            SetImageKey(viewComponentTreeNode, TreeViewImages.Information);
            typesTreeView.Nodes.Add(viewComponentTreeNode);
            AddClassNodesToTreeView(intelliSenseProvider.GetViewComponents(), viewComponentTreeNode);

            TimeSpan elapsedTime = DateTime.Now - startTime;
            elapsedTimeLabel.Text = "Loading took " + elapsedTime.TotalSeconds + " seconds";

            typesTreeView.EndUpdate();

            Cursor = Cursors.Default;
        }

        private void AddClassNodesToTreeView(List<NVClassNode> classNodes, TreeNode rootTreeNode)
        {
            foreach (NVClassNode type in classNodes)
            {
                TreeNode typeTreeNode = new TreeNode(type.Name);
                SetImageKey(typeTreeNode, TreeViewImages.Class);

                TreeNode namespaceTreeNode = new TreeNode(type.FullName);
                SetImageKey(namespaceTreeNode, TreeViewImages.Namespace);
                typeTreeNode.Nodes.Add(namespaceTreeNode);

                foreach (NVMethodNode methodNode in type.Methods)
                {
                    string parametersString = "";
                    foreach (NVParameterNode parameterNode in methodNode.Parameters)
                    {
                        if (parametersString.Length != 0)
                            parametersString += ", ";

                        parametersString += parameterNode.Type.Name + " " + parameterNode.Name;
                    }

                    string methodSignature = string.Format("{0}({1}) : {2}",
                        methodNode.Name,
                        parametersString,
                        methodNode.Type.Name);

                    TreeNode methodTreeNode = new TreeNode(methodSignature);
                    SetImageKey(methodTreeNode, TreeViewImages.Method);

                    typeTreeNode.Nodes.Add(methodTreeNode);
                }

                rootTreeNode.Nodes.Add(typeTreeNode);
            }
        }

        private void SetImageKey(TreeNode treeNode, TreeViewImages image)
        {
            treeNode.ImageKey = image.ToString();
            treeNode.SelectedImageKey = image.ToString();
        }
    }
}