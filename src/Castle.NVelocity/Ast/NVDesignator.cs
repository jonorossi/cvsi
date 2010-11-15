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

namespace Castle.NVelocity.Ast
{
    using System.Collections.Generic;

    public class NVDesignator : NVIdNode
    {
        private NVIdNode _id; // node for identifier
        private List<NVSelector> _selectors = new List<NVSelector>();

        public NVDesignator(string name)
            : base(name)
        {
        }

        public NVDesignator(string name, List<NVSelector> selectors)
            : base(name)
        {
            _selectors = selectors;
        }

        public NVIdNode IdentNode
        {
            get { return _id; }
            set { _id = value; }
        }

        public List<NVSelector> Selectors
        {
            get { return _selectors; }
            set { _selectors = value; }
        }

        public override void DoSemanticChecks(ErrorHandler errs, Scope currentScope)
        {
            _id = currentScope.Find(_name);
            if (_id == null)
            {
                //AddSemanticError(errs, string.Format("The name '{0}' does not exist in the current context", _name),
                //    _pos, ErrorSeverity.Error);
                return;
            }

            _type = _id.Type;
            foreach (NVSelector selector in _selectors)
            {
                _type = selector.DoSemanticChecks(errs, currentScope, _type);
            }
        }

        public override AstNode GetNodeAt(int line, int pos)
        {
            //TODO add support for line detection
            foreach (NVSelector selector in _selectors)
            {
                NVSelector foundSelector = (NVSelector)selector.GetNodeAt(line, pos);
                if (foundSelector != null)
                {
                    return foundSelector;
                }
            }
            return base.GetNodeAt(line, pos);
        }
    }
}