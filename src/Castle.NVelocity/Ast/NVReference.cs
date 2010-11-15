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
    public class NVReference : AstNode
    {
        private readonly NVDesignator _designator;

        public NVReference(NVDesignator designator)
        {
            _designator = designator;
        }

        public NVDesignator Designator
        {
            get { return _designator; }
        }

        public override Position Position
        {
            get { return _designator.Position; }
            set { _designator.Position = value; }
        }

        public override void DoSemanticChecks(ErrorHandler errs, Scope currentScope)
        {
            _designator.DoSemanticChecks(errs, currentScope);
            
            // TODO finish
        }

        public override AstNode GetNodeAt(int line, int pos)
        {
            return _designator.GetNodeAt(line, pos);
        }
    }
}