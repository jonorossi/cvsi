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

namespace Castle.VisualStudio.NVelocityLanguageService
{
    using System.Collections.Generic;
    using Castle.NVelocity.Ast;
    using Microsoft.VisualStudio.Package;
    using MonoRailIntelliSenseProvider;

    public class NVelocityMethods : Methods
    {
        private readonly List<NVMethodNode> _methodNodes;

        public NVelocityMethods(List<NVMethodNode> methodNodes)
        {
            _methodNodes = methodNodes;
        }

        /// <summary>
        /// Returns the name of the specified method signature.
        /// </summary>
        /// <param name="index">The index of the method whose name is to be returned.</param>
        /// <returns>The name of the specified method, or null.</returns>
        public override string GetName(int index)
        {
            return _methodNodes[index].Name;
        }

        /// <summary>
        /// Returns the number of overloaded method signatures represented in this collection.
        /// </summary>
        /// <returns>The number of signatures in the collection.</returns>
        public override int GetCount()
        {
            return _methodNodes.Count;
        }

        /// <summary>
        /// Returns the description of the specified method signature.
        /// </summary>
        /// <param name="index">An index into the internal list to the desired method signature.</param>
        /// <returns>The description of the specified method signature, or null.</returns>
        public override string GetDescription(int index)
        {
            return _methodNodes[index].Name;
        }

        /// <summary>
        /// Returns the return type of the specified method signature.
        /// </summary>
        /// <param name="index">An index into the list of method signatures.</param>
        /// <returns>The return type of the specified method signature, or null.</returns>
        public override string GetType(int index)
        {
            return _methodNodes[index].Type.Name;
        }

        /// <summary>
        /// Returns the number of parameters on the specified method signature.
        /// </summary>
        /// <param name="index">An index into the list of method signatures.</param>
        /// <returns>The number of parameters on the specified method signature, or -1.</returns>
        public override int GetParameterCount(int index)
        {
            return _methodNodes[index].Parameters.Count;
        }

        /// <summary>
        /// Returns information about the specified parameter on the specified method signature.
        /// </summary>
        /// <param name="index">An index into the list of method signatures.</param>
        /// <param name="parameter">An index into the parameter list of the specified method signature.</param>
        /// <param name="name">Returns the name of the parameter.</param>
        /// <param name="display">Returns the parameter name and type formatted for display.</param>
        /// <param name="description">Returns a string containing a description of the parameter.</param>
        public override void GetParameterInfo(int index, int parameter, out string name, out string display,
                                              out string description)
        {
            NVMethodNode methodNode = _methodNodes[index];
            NVParameterNode parameterNode = methodNode.Parameters[parameter];
            NVClassNode classNode = (NVClassNode)methodNode.Parent;
            
            name = parameterNode.Name;
            display = string.Format("{0} {1}", parameterNode.Type.Name, parameterNode.Name);

            // Retrieve XML documentation
            XmlDocumentationProvider documentationProvider =
                new XmlDocumentationProvider(classNode.AssemblyFileName);
            description = documentationProvider.GetParameterDocumentation(
                classNode.FullName, methodNode.Name, parameterNode.Name);

            if (string.IsNullOrEmpty(description))
            {
                description = string.Format("Could not retrieve documentation for parameter '{0}'.",
                    parameterNode.Name);
            }
        }
    }
}