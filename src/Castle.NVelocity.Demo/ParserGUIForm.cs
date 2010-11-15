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

namespace Castle.NVelocity.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using NVelocity.Ast;

    public partial class ParserGUIForm : Form
    {
        private const string _templatePath = @"..\..\GUITemplate.vm";
        private Scanner _scanner;
        private Parser _parser;

        public ParserGUIForm()
        {
            InitializeComponent();

            templateTextBox.Text = File.ReadAllText(_templatePath);

            ParseTemplate();
        }

        private void ParserGuiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(_templatePath, templateTextBox.Text);
        }

        private void templateTextBox_TextChanged(object sender, EventArgs e)
        {
            ParseTemplate();
        }

        private void ParseTemplate()
        {
            errorsListView.Items.Clear();
            templateTextBox.ForeColor = SystemColors.WindowText;
            ErrorHandler errors = new ErrorHandler();
            try
            {
                _scanner = new Scanner(errors);
                _scanner.SetSource(templateTextBox.Text);
                _parser = new Parser(_scanner, errors);
                TemplateNode templateNode = _parser.ParseTemplate();

                BuildParseTree(templateNode);
            }
            catch (Exception ex)
            {
                errorsListView.Items.Add(ex.Message);
                templateTextBox.ForeColor = Color.Red;
            }
        }

        private void BuildParseTree(TemplateNode templateNode)
        {
            parseTreeView.BeginUpdate();
            
            parseTreeView.Nodes.Clear();

            TreeNode templateTreeNode = new TreeNode("Template");
            templateTreeNode.Tag = templateNode;
            parseTreeView.Nodes.Add(templateTreeNode);

            AddNodesToTree(templateNode, templateTreeNode);

            templateTreeNode.ExpandAll();

            parseTreeView.EndUpdate();
        }

        private void AddNodesToTree(AstNode parentAstNode, TreeNode parentTreeNode)
        {
            IList<AstNode> nodes;
            if (parentAstNode is TemplateNode)
            {
                nodes = ((TemplateNode)parentAstNode).Content;
            }
            else if (parentAstNode is XmlElement)
            {
                nodes = ((XmlElement) parentAstNode).Content;
            }
            else
            {
                return;
            }

            foreach (AstNode node in nodes)
            {
                TreeNode treeNode = new TreeNode(node.ToString());
                treeNode.Tag = node;
                parentTreeNode.Nodes.Add(treeNode);
                AddNodesToTree(node, treeNode);
            }
        }

        private void templateTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Stop the UI from updating
            parseTreeView.BeginUpdate();

            // Reset all the tree node fonts
            ResetTreeNodeFont(parseTreeView.Nodes);

            // Colourise tree nodes that are part of the current selection nodes
            int line = templateTextBox.GetLineFromCharIndex(templateTextBox.SelectionStart);
            int pos = templateTextBox.SelectionStart - templateTextBox.GetFirstCharIndexFromLine(line);

            ApplyFormattingToTreeNodes(parseTreeView.Nodes, line + 1, pos + 1);

            // Allow the UI to update
            parseTreeView.EndUpdate();
        }

        private void ResetTreeNodeFont(TreeNodeCollection treeNodes)
        {
            foreach (TreeNode treeNode in treeNodes)
            {
                treeNode.NodeFont = parseTreeView.Font;
                treeNode.ForeColor = Color.Black;
                ResetTreeNodeFont(treeNode.Nodes);
            }
        }

        private void ApplyFormattingToTreeNodes(TreeNodeCollection treeNodes, int line, int pos)
        {
            foreach (TreeNode treeNode in treeNodes)
            {
                AstNode astNode = (AstNode) treeNode.Tag;

                if (astNode != null && astNode.Position != null)
                {
                    if (astNode.Position.Contains(line, pos))
                    {
                        treeNode.ForeColor = Color.Blue;
                        treeNode.NodeFont = new Font(parseTreeView.Font, FontStyle.Bold);
                    }
                }

                ApplyFormattingToTreeNodes(treeNode.Nodes, line, pos);
            }
        }

        private void parseTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AstNode astNode = (AstNode) e.Node.Tag;
            astNodeLabel.Text = string.Format("Type: {0}, Position: {1}",
                astNode.GetType().Name, astNode.Position);

            if (astNode.Position != null)
            {
                templateTextBox.SelectionStart = templateTextBox.GetFirstCharIndexFromLine(astNode.Position.StartLine) +
                                                 astNode.Position.StartPos;
                if (astNode.Position.StartLine == astNode.Position.EndLine)
                {
                    templateTextBox.SelectionLength = astNode.Position.EndPos - astNode.Position.StartPos;
                }
                else
                {
                    templateTextBox.SelectionLength = 1;
                }
                templateTextBox.Focus();
            }
        }
    }
}
