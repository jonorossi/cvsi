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

namespace Castle.VisualStudio.NVelocityLanguageService.DebugWindow
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Castle.NVelocity.Ast;
    using NVelocity;

    public enum TypeImage
    {
        Assembly, Class, Error, Field, Information, Macro, Method, Namespace, Property, Variable, Warning
    }

    public partial class DebugForm : Form
    {
        private delegate void UpdateUIDelegate(int cursorLine, int cursorCol, TemplateNode templateNode);

        private readonly UpdateUIDelegate _updateUIDelegate;

        public DebugForm()
        {
            InitializeComponent();

            Height = Screen.PrimaryScreen.WorkingArea.Height;

            _updateUIDelegate = UpdateUIInUIThread;
        }

        public void UpdateUI(int cursorLine, int cursorCol, TemplateNode templateNode)
        {
            Invoke(_updateUIDelegate, cursorLine, cursorCol, templateNode);
        }

        private void UpdateUIInUIThread(int cursorLine, int cursorCol, TemplateNode templateNode)
        {
            // Current Position
            currentPositionLabel.Text = string.Format("Line: {0}, Pos: {1}", cursorLine, cursorCol);
            
            // Current Node
            AstNode astNode = templateNode.GetNodeAt(cursorLine, cursorCol);
            currentNodeLabel.Text = string.Format("{0}, Pos: {1}",
                astNode != null ? astNode.GetType().Name : "null astNode",
                astNode != null ? astNode.Position.ToString() : "null astNode");

            // Abstract Syntax Tree
            BuildTreeViewFromAst(templateNode);
            ApplyFormattingToTreeNodes(astTreeView.Nodes, cursorLine, cursorCol);

            // Current Scope
            scopeListView.BeginUpdate();
            scopeListView.Items.Clear();
            AddItemsFromScope(templateNode, cursorLine, cursorCol);
            scopeListView.EndUpdate();
        }

        private void BuildTreeViewFromAst(TemplateNode templateNode)
        {
            astTreeView.BeginUpdate();

            astTreeView.Nodes.Clear();

            TreeNode templateTreeNode = new TreeNode("Template");
            templateTreeNode.Tag = templateNode;
            astTreeView.Nodes.Add(templateTreeNode);

            AddNodesToTree(templateNode, templateTreeNode);

            templateTreeNode.ExpandAll();

            astTreeView.EndUpdate();
        }

        private void AddNodesToTree(AstNode parentAstNode, TreeNode parentTreeNode)
        {
            if (parentAstNode is TemplateNode)
            {
                // Template Content
                foreach (AstNode node in ((TemplateNode)parentAstNode).Content)
                {
                    AddNodesToTree(node, parentTreeNode);
                }
            }
            else if (parentAstNode is NVBinaryExpression)
            {
                NVBinaryExpression binExpr = (NVBinaryExpression)parentAstNode;
                TreeNode binExprTreeNode = new TreeNode(string.Format("NVBinaryExpression={{Op:\"{0}\"}}",
                    binExpr.Op));
                binExprTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(binExprTreeNode);

                AddNodesToTree(binExpr.Lhs, binExprTreeNode);
                AddNodesToTree(binExpr.Rhs, binExprTreeNode);
            }
            else if (parentAstNode is NVBoolExpression)
            {
                TreeNode boolTreeNode = new TreeNode(string.Format("NVBoolExpression={{Value:\"{0}\"}}",
                    ((NVBoolExpression)parentAstNode).Value));
                boolTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(boolTreeNode);
            }
            else if (parentAstNode is NVForeachDirective)
            {
                NVForeachDirective foreachDirective = (NVForeachDirective)parentAstNode;
                TreeNode foreachTreeNode = new TreeNode(string.Format("NVForeachDirective={{Iterator:\"{0}\"}}",
                    foreachDirective.Iterator));
                foreachTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(foreachTreeNode);

                // Content
                foreach (AstNode astNode in foreachDirective.Content)
                {
                    AddNodesToTree(astNode, foreachTreeNode);
                }
            }
            else if (parentAstNode is NVDirective)
            {
                TreeNode directiveTreeNode = new TreeNode(string.Format("NVDirective={{Name:\"{0}\"}}",
                    ((NVDirective)parentAstNode).Name));
                directiveTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(directiveTreeNode);
            }
            else if (parentAstNode is NVNumExpression)
            {
                TreeNode numTreeNode = new TreeNode(string.Format("NVNumExpression={{Value:\"{0}\"}}",
                    ((NVNumExpression)parentAstNode).Value));
                numTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(numTreeNode);
            }
            else if (parentAstNode is NVReference)
            {
                TreeNode referenceTreeNode = new TreeNode(string.Format("NVReference={{Name:\"{0}\"}}",
                    ((NVReference)parentAstNode).Designator.Name));
                referenceTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(referenceTreeNode);

                foreach (NVSelector selector in ((NVReference)parentAstNode).Designator.Selectors)
                {
                    AddNodesToTree(selector, referenceTreeNode);
                }
            }
            else if (parentAstNode is NVSelector)
            {
                NVSelector selector = (NVSelector)parentAstNode;
                TreeNode selectorTreeNode = new TreeNode(string.Format("NVSelector={{Name:\"{0}\", Type:\"{1}\"}}",
                    selector.Name, selector.Type != null ? selector.Type.Name : ""));
                selectorTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(selectorTreeNode);

                if (selector.Actuals.Count > 0)
                {
                    TreeNode actualsTreeNode = new TreeNode("Actuals:");
                    foreach (NVExpression actual in selector.Actuals)
                    {
                        AddNodesToTree(actual, actualsTreeNode);
                    }
                    selectorTreeNode.Nodes.Add(actualsTreeNode);
                }
            }
            else if (parentAstNode is NVStringExpression)
            {
                TreeNode stringTreeNode = new TreeNode(string.Format("NVStringExpression={{Value:\"{0}\"}}",
                    ((NVStringExpression)parentAstNode).Value));
                stringTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(stringTreeNode);
            }
            else if (parentAstNode is XmlAttribute)
            {
                TreeNode attributeTreeNode = new TreeNode(string.Format("XmlAttribute={{Name:\"{0}\"}}",
                    ((XmlAttribute)parentAstNode).Name));
                attributeTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(attributeTreeNode);

                // XML Attribute Content
                foreach (AstNode node in ((XmlAttribute)parentAstNode).Content)
                {
                    AddNodesToTree(node, attributeTreeNode);
                }
            }
            else if (parentAstNode is XmlElement)
            {
                XmlElement xmlElement = (XmlElement)parentAstNode;
                TreeNode elementTreeNode = new TreeNode(string.Format("XmlElement={{Name:\"{0}\"}}",
                    xmlElement.Name));
                elementTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(elementTreeNode);

                // XML Element Attributes
                TreeNode attributesTreeNode = new TreeNode("Attributes:");
                foreach (AstNode attribute in xmlElement.Attributes)
                {
                    AddNodesToTree(attribute, attributesTreeNode);
                }
                elementTreeNode.Nodes.Add(attributesTreeNode);

                // XML Element Content
                TreeNode contentTreeNode = new TreeNode("Content:");
                foreach (AstNode node in xmlElement.Content)
                {
                    AddNodesToTree(node, contentTreeNode);
                }
                elementTreeNode.Nodes.Add(contentTreeNode);
            }
            else if (parentAstNode is XmlTextNode)
            {
                TreeNode textNodeTreeNode = new TreeNode(string.Format("XmlTextNode={{Text:\"{0}\"}}",
                    ((XmlTextNode)parentAstNode).Text.Replace("\n", "\\n")));
                textNodeTreeNode.Tag = parentAstNode;
                parentTreeNode.Nodes.Add(textNodeTreeNode);
            }
            else
            {
                if (parentAstNode != null)
                {
                    TreeNode treeNode = new TreeNode("UNKNOWN===" + parentAstNode.GetType().Name);
                    treeNode.Tag = parentAstNode;
                    parentTreeNode.Nodes.Add(treeNode);
                }
            }
        }

        private void ApplyFormattingToTreeNodes(TreeNodeCollection treeNodes, int line, int pos)
        {
            foreach (TreeNode treeNode in treeNodes)
            {
                try
                {
                    AstNode astNode = (AstNode)(treeNode.Tag);

                    if (astNode != null && astNode.Position != null)
                    {
                        if (astNode.Position.Contains(line, pos))
                        {
                            treeNode.ForeColor = Color.Blue;
                            //treeNode.NodeFont = new Font(astTreeView.Font, FontStyle.Bold);
                        }
                    }
                }
                catch (Exception)
                {
                }

                ApplyFormattingToTreeNodes(treeNode.Nodes, line, pos);
            }
        }

        private void AddItemsFromScope(TemplateNode templateNode, int cursorLine, int cursorCol)
        {
            Scope scope = templateNode.GetScopeAt(cursorLine, cursorCol);

            while (scope != null)
            {
                foreach (KeyValuePair<string, NVIdNode> identifier in scope.GetIdentifiers())
                {
                    ListViewItem item = new ListViewItem(identifier.Value.Name);
                    if (identifier.Value.Type != null)
                        item.SubItems.Add(identifier.Value.Type.Name);

                    if (identifier.Value is NVClassNode)
                        item.ImageKey = TypeImage.Class.ToString();
                    else if (identifier.Value is NVLocalNode)
                        item.ImageKey = TypeImage.Variable.ToString();
                    else
                        item.ImageKey = TypeImage.Error.ToString();

                    scopeListView.Items.Add(item);
                }

                scope = scope.OuterScope;

                if (scope != null)
                {
                    scopeListView.Items.Add("--- Parent Scope ---");
                }
            }
        }
    }
}