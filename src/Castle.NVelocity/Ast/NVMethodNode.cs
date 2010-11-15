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

    public class NVMethodNode : NVIdNode
    {
        private List<NVParameterNode> _parameters;

        public NVMethodNode(string name)
            : base(name)
        {
        }

        public NVMethodNode(/*Attribute att,*/ string name, NVTypeNode returnType/*, List<ParNode> parameters*/)
            : base(name)
        {
            _type = returnType;
        }

        public List<NVParameterNode> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public NVIdNode Parent
        {
            get { return _parent; }
            set { _parent = (NVClassNode)value; }
        }
    }
}