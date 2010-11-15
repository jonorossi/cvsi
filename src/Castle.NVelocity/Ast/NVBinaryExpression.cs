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
    public class NVBinaryExpression : NVExpression
    {
        private readonly Operator _op;
        private readonly NVExpression _lhs;
        private readonly NVExpression _rhs;

        public NVBinaryExpression(Operator op, NVExpression lhs, NVExpression rhs)
        {
            _op = op;
            _rhs = rhs;
            _lhs = lhs;
        }

        public Operator Op
        {
            get { return _op; }
        }

        public NVExpression Lhs
        {
            get { return _lhs; }
        }

        public NVExpression Rhs
        {
            get { return _rhs; }
        }
    }
}
