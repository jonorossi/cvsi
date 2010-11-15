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

namespace Castle.NVelocity
{
    using System.Collections.Generic;
    using Ast;

    public class Scope
    {
        private readonly Dictionary<string, NVIdNode> _scope = new Dictionary<string, NVIdNode>();
        private readonly Scope _outerScope;
        private readonly AstNode _owner;

        /// <summary>
        /// Creates a new scope linked to the existing scope.
        /// </summary>
        /// <param name="outerScope">The scope that encloses this scope.</param>
        /// <param name="owner">The owner identifier node.</param>
        public Scope(Scope outerScope, AstNode owner)
        {
            _outerScope = outerScope;
            _owner = owner;
        }

        /// <summary>
        /// Looks up an identifier in the scope. This check enclosing scopes.
        /// </summary>
        public NVIdNode Find(string name)
        {
            if (_scope.ContainsKey(name))
            {
                return _scope[name];
            }

            if (_outerScope != null)
            {
                return _outerScope.Find(name);
            }

            return null;
        }

        /// <summary>
        /// Determines if an identifier exists in this scope. Does not check enclosing scopes.
        /// </summary>
        public bool Exists(string name)
        {
            return _scope.ContainsKey(name);
        }

        /// <summary>
        /// Adds an identifier to the current scope.
        /// </summary>
        public void Add(NVIdNode id)
        {
            _scope.Add(id.Name, id);
        }

        /// <summary>
        /// Gets the enclosing scope for this scope.
        /// </summary>
        public Scope OuterScope
        {
            get { return _outerScope; }
        }

//        public MethodNode GetMethodOwner()
//        {
//            if (owner is MethodNode) return (MethodNode)owner;
//            return null;
//        }
//
//        public ClassNode GetClassOwner()
//        {
//            if (owner is MethodNode) return ((MethodNode)owner).Parent;
//            return (ClassNode)owner;
//        }

        /// <summary>
        /// Provides an iterator method for iterating over the identifiers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, NVIdNode> GetIdentifiers()
        {
            return _scope;
        }
    }
}