// Copyright 2007-2010 Jonathon Rossi - http://jonorossi.com/
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

namespace Castle.VisualStudio.NVelocityLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Castle.NVelocity.Ast;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.Win32;
    using MonoRailIntelliSenseProvider;

    public class NVelocityAuthoringScope : AuthoringScope
    {
        private readonly TemplateNode _templateNode;
        private string _fileName;

        public NVelocityAuthoringScope(TemplateNode templateNode, string fileName)
        {
            _templateNode = templateNode;
            _fileName = fileName;
        }

        public override Declarations GetDeclarations(IVsTextView view, int line, int col,
            TokenInfo info, ParseReason reason)
        {
            // If the intellisense parse reason was called before the template could be parsed
            if (_templateNode == null)
            {
                return null;
            }

            AstNode astNode = _templateNode.GetNodeAt(line + 1, col + 1);
            if (astNode == null)
            {
                Debug.Fail("Null AstNode when attempting to provide IntelliSense in NVelocityAuthoringScope.");
                return null;
            }

            NVelocityDeclarations declarations = new NVelocityDeclarations();

            if (astNode is NVDesignator)
            {
                List<NVLocalNode> localNodes = _templateNode.GetLocalNodesFromScope(line, col);
                localNodes.Sort(new Comparison<NVLocalNode>(
                    delegate(NVLocalNode x, NVLocalNode y)
                    {
                        return x.Name.CompareTo(y.Name);
                    }));
                foreach (NVLocalNode localNode in localNodes)
                {
                    declarations.Add(new NVelocityDeclaration(localNode.Name, localNode, IntelliSenseIcon.Variable));
                }
            }
            else if (astNode is NVSelector)
            {
                NVTypeNode typeNode = ((NVSelector)astNode).GetParentType();
                if (typeNode is NVClassNode)
                {
                    NVClassNode classNode = (NVClassNode)typeNode;

                    Dictionary<string, NVMethodNode> uniqueMethods = new Dictionary<string, NVMethodNode>();
                    foreach (NVMethodNode methodNode in classNode.Methods)
                    {
                        if (!uniqueMethods.ContainsKey(methodNode.Name))
                        {
                            uniqueMethods.Add(methodNode.Name, methodNode);
                        }
                    }

                    List<NVMethodNode> uniqueMethodsList = new List<NVMethodNode>();
                    foreach (KeyValuePair<string, NVMethodNode> pair in uniqueMethods)
                    {
                        uniqueMethodsList.Add(pair.Value);
                    }

                    uniqueMethodsList.Sort(new Comparison<NVMethodNode>(
                        delegate(NVMethodNode x, NVMethodNode y)
                        {
                             return x.Name.CompareTo(y.Name);
                        }));
                    foreach (NVMethodNode methodNode in uniqueMethodsList)
                    {
                        declarations.Add(new NVelocityDeclaration(methodNode.Name, methodNode, IntelliSenseIcon.Method));
                    }
                }
                else
                {
                    if (typeNode == null)
                    {
                        declarations.Add(new NVelocityDeclaration("Error: TypeNode is null", null, IntelliSenseIcon.Error));
                    }
                    else
                    {
                        declarations.Add(new NVelocityDeclaration("Error: Unsupported type for NVSelector " + typeNode.GetType().Name,
                            null, IntelliSenseIcon.Error));
                    }
                }
            }
            else if (astNode is NVDirective)
            {
                NVDirective nvDirective = (NVDirective)astNode;
                if ((col + 1) - astNode.Position.StartPos == 1)
                {
                    // TODO: change the if expression so that it checks if the col is between the # and (
                    //       because you can bring up the intellisense list in the middle of the identifier
                    //       and it should display the list of the directives instead of the view components.
                    List<string> directivesList = new List<string>();
                    directivesList.AddRange(new string[]
                        {
                            "if", "elseif", "else", "end", "foreach", "set", "stop", "component",
                            "blockcomponent", "literal", "macro"
                        });
                    directivesList.Sort();

                    foreach (string directive in directivesList)
                    {
                        declarations.Add(new NVelocityDeclaration(directive, null, IntelliSenseIcon.Macro));
                    }
                }
                else if (nvDirective.Name == "component" || nvDirective.Name == "blockcomponent")
                {
                    List<NVClassNode> viewComponents = _templateNode.GetViewComponentsFromScope();
                    viewComponents.Sort(new Comparison<NVClassNode>(
                        delegate(NVClassNode x, NVClassNode y)
                        {
                            return x.Name.CompareTo(y.Name);
                        }));
                    foreach (NVClassNode classNode in viewComponents)
                    {
                        declarations.Add(new NVelocityDeclaration(classNode.Name, classNode, IntelliSenseIcon.Class));
                    }
                }
            }
            else if (astNode is XmlElement)
            {
                string xhtmlSchemaFileName = GetXhtmlSchemaFileName();
                if (string.IsNullOrEmpty(xhtmlSchemaFileName))
                {
                    Debug.Fail("Could not find XHTML schema.");
                    return declarations;
                }
                XhtmlSchemaProvider xhtmlSchemaProvider = new XhtmlSchemaProvider(xhtmlSchemaFileName);

                XmlElement xmlElement = (XmlElement)astNode;
                if (string.IsNullOrEmpty(xmlElement.Name))
                {
                    if (xmlElement.Parent != null)
                    {
                        if (!xmlElement.Parent.IsSelfClosing && !xmlElement.IsComplete && !xmlElement.Parent.IsComplete)
                        {
                            declarations.Add(new NVelocityDeclaration(string.Format("/{0}>", xmlElement.Parent.Name),
                                null, IntelliSenseIcon.XmlElement));
                        }
                    }

                    foreach (string xhtmlElement in xhtmlSchemaProvider.GetElements())
                    {
                        declarations.Add(new NVelocityDeclaration(xhtmlElement, null, IntelliSenseIcon.XmlElement));
                    }
                }
                else
                {
                    // Retrieve attributes
                    List<string> xhtmlAttributes = xhtmlSchemaProvider.GetAttributes(xmlElement.Name);

                    // Remove attributes that are already used
                    foreach (AstNode attribute in xmlElement.Attributes)
                    {
                        if (attribute is XmlAttribute)
                        {
                            XmlAttribute xmlAttribute = (XmlAttribute)attribute;
                            if (xhtmlAttributes.Contains(xmlAttribute.Name))
                            {
                                xhtmlAttributes.Remove(xmlAttribute.Name);
                            }
                        }
                    }

                    // Add the declarations for the attributes to show
                    foreach (string xhtmlAttribute in xhtmlAttributes)
                    {
                        declarations.Add(new NVelocityDeclaration(xhtmlAttribute, null, IntelliSenseIcon.XmlAttribute));
                    }
                }
            }
            else
            {
                declarations.Add(new NVelocityDeclaration("Error: Context unknown, type is " + astNode.GetType().Name,
                    null, IntelliSenseIcon.Error));
            }

            return declarations;
        }

        private static string GetXhtmlSchemaFileName()
        {
#if VS2005
            const string vsSetupKeyPath = @"SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS";
#elif VS2008
            const string vsSetupKeyPath = @"SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VS";
#elif VS2010
            const string vsSetupKeyPath = @"SOFTWARE\Microsoft\VisualStudio\10.0\Setup\VS";
#elif VS2012
            const string vsSetupKeyPath = @"SOFTWARE\Microsoft\VisualStudio\11.0\Setup\VS";
#elif VS2013
            const string vsSetupKeyPath = @"SOFTWARE\Microsoft\VisualStudio\12.0\Setup\VS";
#else
#error Unsupported Visual Studio version
#endif

            RegistryKey vsSetupKey = Registry.LocalMachine.OpenSubKey(vsSetupKeyPath);
            string productDir = vsSetupKey.GetValue("ProductDir") as string;
            if (!string.IsNullOrEmpty(productDir))
            {
                return Path.Combine(productDir, @"Xml\Schemas\xhtml.xsd");
            }

            return null;
        }

        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        public override Methods GetMethods(int line, int col, string name)
        {
            AstNode node = _templateNode.GetNodeAt(line + 1, col + 1);
            if (node is NVSelector)
            {
                NVClassNode classNode = ((NVClassNode)((NVSelector)node).GetParentType());

                if (classNode == null)
                {
                    return null;
                }

                List<NVMethodNode> overloadedMethods = new List<NVMethodNode>();
                foreach (NVMethodNode methodNode in classNode.Methods)
                {
                    if (methodNode.Name == ((NVSelector)node).Name)
                    {
                        overloadedMethods.Add(methodNode);
                    }
                }

                if (overloadedMethods.Count > 0)
                {
                    return new NVelocityMethods(overloadedMethods);
                }
            }
            return null;
        }

        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }
    }
}