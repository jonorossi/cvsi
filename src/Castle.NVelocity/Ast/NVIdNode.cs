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
    public abstract class NVIdNode : AstNode
    {
        protected string _name;
        protected NVClassNode _parent; // the class which owns this id
        protected NVTypeNode _type; // the- type of this id

        public NVIdNode(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public virtual NVTypeNode Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}