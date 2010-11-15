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

    public class XmlAttribute : AstNode
    {
        private readonly string _name;
        private readonly List<AstNode> _content = new List<AstNode>();

        public XmlAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public List<AstNode> Content
        {
            get { return _content; }
        }

        public override void DoSemanticChecks(ErrorHandler errs, Scope currentScope)
        {
            foreach (AstNode astNode in _content)
            {
                astNode.DoSemanticChecks(errs, currentScope);
            }
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