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

namespace Castle.NVelocity.Ast
{
    using System;
    using System.Collections.Generic;

    public enum NVClassNodePurpose
    {
        Helper, ViewComponent
    }

    public class NVClassNode : NVTypeNode
    {
        private string _fullName;
        private readonly List<NVMethodNode> _methods = new List<NVMethodNode>();
        private Scope _scope;
        private NVClassNodePurpose _classPurpose;
        private string _assemblyFileName;

        public NVClassNode(string name, string fullName)
            : base(name)
        {
            _fullName = fullName;
        }

        public override NVTypeNode Type
        {
            get { return this; }
            set
            {
                throw new NotSupportedException(
                    string.Format("The type of a {0} cannot be set.", GetType().Name));
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public NVClassNodePurpose ClassPurpose
        {
            get { return _classPurpose; }
            set { _classPurpose = value; }
        }

        public string AssemblyFileName
        {
            get { return _assemblyFileName; }
            set { _assemblyFileName = value; }
        }

        public void AddMethod(NVMethodNode methodNode)
        {
            methodNode.Parent = this;
            _methods.Add(methodNode);
        }

        public void AddIdentsToScope(ErrorHandler errs, Scope currentScope)
        {
            _scope = new Scope(currentScope, this);

            foreach (NVMethodNode methodNode in _methods)
            {
                if (_scope.Exists(methodNode.Name))
                {
                    AddSemanticError(errs, string.Format("Type '{0}' already defines a member called {1}", _name, methodNode.Name),
                        methodNode.Position, ErrorSeverity.Error);
                }
                else
                {
                    _scope.Add(methodNode);
                }
            }
        }

        public NVIdNode FindMember(ErrorHandler errs, Position pos, string name)
        {
            if (_scope != null)
            {
                NVIdNode idNode = _scope.Find(name);
                if (idNode != null)
                {
                    return idNode;
                }
            }

            // Could not obtain the identifier node
            AddSemanticError(errs, string.Format(
                "The name '{0}' does not exist in the current context", name), _pos, ErrorSeverity.Warning);
            return null;
        }

        public List<NVMethodNode> Methods
        {
            get { return _methods; }
        }
    }
}