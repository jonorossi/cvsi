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

namespace Castle.VisualStudio.MonoRailIntelliSenseProvider
{
    using System.Collections.Generic;
    using System.IO;
    using Castle.NVelocity.Ast;
    using Mono.Cecil;

    public class IntelliSenseProvider
    {
        private const string AbstractHelperTypeName = "Castle.MonoRail.Framework.Helpers.AbstractHelper";
        private const string ViewComponentTypeName = "Castle.MonoRail.Framework.ViewComponent";
        private const string AssemblyFileNamePattern = "*.dll";
        private const string BinaryDirectoryName = "bin";
        private const string BinaryDebugDirectoryName = "Debug";

        private readonly string _binaryDirectory;

        private static readonly List<string> _ignoreAssemblies = new List<string>();

        static IntelliSenseProvider()
        {
            _ignoreAssemblies.AddRange(new string[] {
                "Castle.ActiveRecord.dll",
                "Castle.Components.Binder.dll",
                "Castle.Components.Common.EmailSender.dll",
                "Castle.Components.Validator.dll",
                "Castle.Core.dll",
                "Castle.DynamicProxy.dll",
                "Castle.MonoRail.Framework.Views.NVelocity.dll",
                "Iesi.Collections.dll",
                "log4net.dll",
                "NHibernate.dll",
                "NVelocity.dll"
            });
        }

        public IntelliSenseProvider(string binaryDirectory)
        {
            _binaryDirectory = binaryDirectory;
        }

        public static string FindBinaryDirectory(string hintViewFileName)
        {
			string webAppRoot = Path.GetDirectoryName(hintViewFileName);

			// Iterate through parent directories looking for the bin directory
			while (true)
			{
				// If found the bin directory then return
				string binDirectory = Path.Combine(webAppRoot, BinaryDirectoryName);
				if (Directory.Exists(binDirectory))
				{
					// If the bin directory contains a debug directory use it instead
					string binDebugDirectory = Path.Combine(binDirectory, BinaryDebugDirectoryName);
					if (Directory.Exists(binDebugDirectory))
					{
						return binDebugDirectory;
					}
					return binDirectory;
				}

				// Otherwise move to the parent directory
				DirectoryInfo parent = Directory.GetParent(webAppRoot);
				if (parent == null)
				{
					return null;
				}
				webAppRoot = parent.FullName;
			}
		}

        public List<NVClassNode> GetHelpers()
        {
            List<string> assemblies = GetAssembliesInBinaryDirectory();
            List<NVClassNode> types = new List<NVClassNode>();
            foreach (string assemblyFileName in assemblies)
            {
                if (!IsIgnored(assemblyFileName))
                {
                    AssemblyDefinition assemblyDefinition = AssemblyFactory.GetAssembly(assemblyFileName);
					TypeDefinitionCollection typeDefs = assemblyDefinition.MainModule.Types;
					foreach (TypeDefinition type in typeDefs)
                    {
                        if (type.Name != "<Module>" &&
							!type.IsAbstract &&
							type.BaseType != null &&
							TypeInherits(typeDefs, type, AbstractHelperTypeName))
                        {
                            NVClassNode classNode = new NVClassNode(type.Name, type.FullName);
                            classNode.ClassPurpose = NVClassNodePurpose.Helper;
                            classNode.AssemblyFileName = assemblyFileName;

                            foreach (MethodDefinition method in type.Methods)
                            {
                                // Properties have the special name IL flag so ignore get_ and set_ methods
                                if ((method.Attributes & MethodAttributes.Public) == MethodAttributes.Public &&
                                    method.SemanticsAttributes != MethodSemanticsAttributes.Getter &&
                                    method.SemanticsAttributes != MethodSemanticsAttributes.Setter)
                                {
                                    NVMethodNode methodNode = new NVMethodNode(method.Name, new NVClassNode(
                                        method.ReturnType.ReturnType.Name, method.ReturnType.ReturnType.FullName));

                                    List<NVParameterNode> parameters = new List<NVParameterNode>();
                                    foreach (ParameterDefinition parameter in method.Parameters)
                                    {
                                        parameters.Add(new NVParameterNode(
                                            parameter.Name,
                                            new NVClassNode(parameter.ParameterType.Name,
                                                parameter.ParameterType.FullName),
                                            parameter.Sequence));
                                    }
                                    methodNode.Parameters = parameters;

                                    classNode.AddMethod(methodNode);
                                }
                            }
                            types.Add(classNode);
                        }
                    }
                }
            }

            return types;
        }

        public List<NVClassNode> GetViewComponents()
        {
            List<string> assemblies = GetAssembliesInBinaryDirectory();
            List<NVClassNode> types = new List<NVClassNode>();
            foreach (string assemblyFileName in assemblies)
            {
                if (!IsIgnored(assemblyFileName))
                {
                    AssemblyDefinition assemblyDefinition = AssemblyFactory.GetAssembly(assemblyFileName);
					TypeDefinitionCollection typeDefs = assemblyDefinition.MainModule.Types;
					foreach (TypeDefinition type in typeDefs)
                    {
						if (type.Name != "<Module>" &&
							!type.IsAbstract &&
							type.BaseType != null &&
							TypeInherits(typeDefs, type, ViewComponentTypeName))
                        {
                            NVClassNode classNode = new NVClassNode(type.Name, type.FullName);
                            classNode.ClassPurpose = NVClassNodePurpose.ViewComponent;
                            classNode.AssemblyFileName = assemblyFileName;
                            types.Add(classNode);
                        }
                    }
                }
            }

            return types;
        }

		private static bool TypeInherits(TypeDefinitionCollection typeDefs, TypeDefinition type, string fullTypeName)
		{
			TypeDefinition currentType = type;

			while (currentType != null && currentType.BaseType != null)
			{
				if (currentType.BaseType.FullName == fullTypeName)
				{
					return true;
				}
				currentType = typeDefs[currentType.BaseType.FullName];
			}

			return false;
		}

        private static bool IsIgnored(string fileName)
        {
            return _ignoreAssemblies.Contains(Path.GetFileName(fileName));
        }

        private List<string> GetAssembliesInBinaryDirectory()
        {
            List<string> assemblies = new List<string>();

            if (_binaryDirectory != null)
            {
                string[] files = Directory.GetFiles(_binaryDirectory, AssemblyFileNamePattern);
                foreach (string fileName in files)
                {
                    assemblies.Add(fileName);
                }
            }

            return assemblies;
        }
    }
}