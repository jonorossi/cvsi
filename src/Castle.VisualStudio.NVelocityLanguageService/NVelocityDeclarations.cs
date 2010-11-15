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
    using System.Collections.Generic;
    using Castle.NVelocity.Ast;
    using Microsoft.VisualStudio.Package;
    using MonoRailIntelliSenseProvider;

    public class NVelocityDeclarations : Declarations
    {
        private readonly IList<NVelocityDeclaration> _declarations = new List<NVelocityDeclaration>();

        public void Add(NVelocityDeclaration declaration)
        {
            _declarations.Add(declaration);
        }

        public override int GetCount()
        {
            return _declarations.Count;
        }

        public override string GetDisplayText(int index)
        {
            string text = "";
            if (index >= 0 && index < _declarations.Count)
            {
                text = _declarations[index].Name;
            }
            return text;
        }

        public override string GetName(int index)
        {
            return GetDisplayText(index);
        }

        public override string GetDescription(int index)
        {
            string description = string.Empty;

            NVIdNode idNode = _declarations[index].IdNode;
            if (idNode is NVClassNode)
            {
                NVClassNode classNode = (NVClassNode)idNode;
                XmlDocumentationProvider documentationProvider =
                    new XmlDocumentationProvider(classNode.AssemblyFileName);
                string classSummary = documentationProvider.GetTypeDocumentation(classNode.FullName);

                if (!string.IsNullOrEmpty(classSummary))
                {
                    description = string.Format("class {0}\n{1}", classNode.FullName, classSummary);
                }
                else
                {
                    description = string.Format("class {0}", classNode.FullName);
                }
            }
            else if (idNode is NVLocalNode)
            {
                NVClassNode classNode = (NVClassNode)idNode.Type;
                if (classNode != null)
                {
                    XmlDocumentationProvider documentationProvider =
                        new XmlDocumentationProvider(classNode.AssemblyFileName);
                    description = documentationProvider.GetTypeDocumentation(classNode.FullName);
                }
            }
            else if (idNode is NVMethodNode)
            {
                NVClassNode classNode = (NVClassNode)((NVMethodNode)idNode).Parent;
                if (classNode != null)
                {
                    XmlDocumentationProvider documentationProvider =
                        new XmlDocumentationProvider(classNode.AssemblyFileName);
                    description = documentationProvider.GetMethodDocumentation(classNode.FullName, idNode.Name);
                }
            }

            // Return the documentation or an error message
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            return string.Format("Could not retrieve documentation for AST node '{0}' (because it is not supported).",
                idNode != null ? idNode.GetType().Name : "");
        }

        public override int GetGlyph(int index)
        {
            return (int)_declarations[index].Icon;
        }
    }
}