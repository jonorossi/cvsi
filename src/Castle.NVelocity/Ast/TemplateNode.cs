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

    public class TemplateNode : AstNode
    {
        private readonly IList<AstNode> _content = new List<AstNode>();
        private readonly IList<NVClassNode> _classes = new List<NVClassNode>();
        private readonly Scope _scope = new Scope(null, null);

        public IList<AstNode> Content
        {
            get { return _content; }
        }

        public Scope Scope
        {
            get { return _scope; }
        }

        public void Add(AstNode node)
        {
            _content.Add(node);
        }

        public void AddRange(List<AstNode> nodes)
        {
            foreach (AstNode astNode in nodes)
            {
                _content.Add(astNode);
            }
        }

        public void AddClass(NVClassNode classNode)
        {
            _classes.Add(classNode);
        }

        public void AddVariable(NVLocalNode localNode)
        {
            _scope.Add(localNode);
        }

        public List<NVLocalNode> GetLocalNodesFromScope(int line, int pos)
        {
            List<NVLocalNode> localNodes = new List<NVLocalNode>();

            Scope scope = GetScopeAt(line, pos);

            while (scope != null)
            {
                foreach (KeyValuePair<string, NVIdNode> pair in scope.GetIdentifiers())
                {
                    if (pair.Value is NVLocalNode)
                    {
                        localNodes.Add((NVLocalNode)pair.Value);
                    }
                }

                scope = scope.OuterScope;
            }
            return localNodes;
        }

        public List<NVClassNode> GetViewComponentsFromScope()
        {
            List<NVClassNode> viewComponents = new List<NVClassNode>();
            foreach (KeyValuePair<string, NVIdNode> pair in _scope.GetIdentifiers())
            {
                if (pair.Value is NVClassNode &&
                    ((NVClassNode)pair.Value).ClassPurpose == NVClassNodePurpose.ViewComponent)
                {
                    viewComponents.Add((NVClassNode)pair.Value);
                }
            }
            return viewComponents;
        }

        public override Scope GetScopeAt(int line, int pos)
        {
            foreach (AstNode astNode in _content)
            {
                Scope foundScope = astNode.GetScopeAt(line, pos);
                if (foundScope != null)
                {
                    return foundScope;
                }
            }
            return _scope;
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

        public void DoSemanticChecks(ErrorHandler errs)
        {
            // Add the classes to the template's scope
            foreach (NVClassNode classNode in _classes)
            {
                if (Scope.Exists(classNode.Name))
                {
                    AddSemanticError(errs, string.Format(
                        "The template already contains a definition for the class '{0}'", classNode.Name),
                        classNode.Position, ErrorSeverity.Warning);
                }
                else
                {
                    _scope.Add(classNode);
                }
            }

            // Add all the class fields and methods to the class' scope
            foreach (NVClassNode classNode in _classes)
            {
                classNode.AddIdentsToScope(errs, _scope);
            }

            // Perform semantic checks on all the AST nodes
            foreach (AstNode astNode in _content)
            {
                astNode.DoSemanticChecks(errs, _scope);
            }
        }
    }
}