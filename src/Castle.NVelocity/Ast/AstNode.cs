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
    public abstract class AstNode
    {
        protected Position _pos;

        /// <summary>
        /// Get and Set the source file position for this node.
        /// </summary>
        public virtual Position Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        //TODO: remove this method because only statements should have it, maybe?
        public virtual void DoSemanticChecks(ErrorHandler errs, Scope currentScope)
        {
//            throw new NotSupportedException("DoSemanticChecks() is not supported on node type " + GetType().Name);
        }

        public virtual Scope GetScopeAt(int line, int pos)
        {
            return null;
        }

        public virtual AstNode GetNodeAt(int line, int pos)
        {
            //throw new NotSupportedException("GetNodeAt() is not supported on node type " + GetType().Name);
            if (_pos == null)
                return null;

            bool inRange = false;

            // Check the lower end of the node's location
            if (line > _pos.StartLine)
                inRange = true;
            else if (line == _pos.StartLine && pos >= _pos.StartPos)
                inRange = true;

            // Return now because the pointer location is outside the lower end of the node's location
            if (!inRange)
                return null;

            // Clear the in range flag because we know the lower end is in range
            inRange = false;

            // Check the upper end of the node's location
            if (line < _pos.EndLine)
                inRange = true;
            else if (line == _pos.EndLine && pos <= _pos.EndPos)
                inRange = true;

            // Return now because the pointer location is outside the upper end of the node's location
            if (!inRange)
                return null;

            // The node must be in the range
            return this;
        }

        protected static void AddSemanticError(ErrorHandler errs, string message, Position pos, ErrorSeverity severity)
        {
            if (pos != null)
            {
                errs.AddError(string.Format("Semantic error on line {0}: {1}", pos.StartLine, message),
                    pos, severity);
            }
            else
            {
                errs.AddError(string.Format("Semantic error: {0}", message), null, severity);
            }
        }
    }
}