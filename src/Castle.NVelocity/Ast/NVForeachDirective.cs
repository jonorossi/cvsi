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
    using System.Collections.Generic;

    public class NVForeachDirective : NVDirective
    {
        private string _iterator;
        private NVExpression _collection;
        private readonly List<AstNode> _content = new List<AstNode>();
        private Scope _scope = new Scope(null, null);

        public NVForeachDirective()
            : base("foreach")
        {
        }

        public string Iterator
        {
            get { return _iterator; }
            set { _iterator = value; }
        }

        public NVExpression Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        public List<AstNode> Content
        {
            get { return _content; }
        }

        public Scope Scope
        {
            get { return _scope; }
        }

        public override void DoSemanticChecks(ErrorHandler errs, Scope currentScope)
        {
            _scope = new Scope(currentScope, this);

            // Add foreach loop iterator variable to the scope
            if (!string.IsNullOrEmpty(_iterator))
            {
                _scope.Add(new NVLocalNode(_iterator, null));
            }

            foreach (AstNode astNode in _content)
            {
                astNode.DoSemanticChecks(errs, _scope);
            }
        }

        public override Scope GetScopeAt(int line, int pos)
        {
            // If this directive contains the line+pos return the scope
            if (_pos.Contains(line, pos))
            {
                return _scope;
            }
            return null;
        }

        public override AstNode GetNodeAt(int line, int pos)
        {
            foreach (AstNode astNode in _content)
            {
                AstNode foundNode = astNode.GetNodeAt(line, pos);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }
    }
}