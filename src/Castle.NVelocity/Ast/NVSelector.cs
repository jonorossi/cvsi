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

    public class NVSelector : AstNode
    {
        private NVIdNode _id;
        private NVTypeNode _type;
        private List<NVExpression> _actuals = new List<NVExpression>();
        private readonly NVReference _nvReference;

        public NVSelector(NVIdNode id, NVReference nvReference)
        {
            _id = id;
            _nvReference = nvReference;
        }

        public string Name
        {
            get { return _id != null ? _id.Name : ""; }
        }

        public NVTypeNode Type
        {
            get { return _type; }
        }

        public List<NVExpression> Actuals
        {
            get { return _actuals; }
            set { _actuals = value; }
        }

        public NVReference NVReference
        {
            get { return _nvReference; }
        }

        public NVTypeNode DoSemanticChecks(ErrorHandler errs, Scope currentScope, NVTypeNode currentType)
        {
            currentType = ResolveNamedTypeNodes(errs, currentScope, currentType);

            // Check to see if the current node is an empty placeholder selector
            if (_id.Name == "")
            {
                return currentType;
            }

            if (currentType == null)
            {
                //TODO: Uncomment this when support for objects passed through the property bag is much better
                //AddSemanticError(errs, "Type of identifier unknown", _pos, ErrorSeverity.Warning);
                return null;
            }

            if (!(currentType is NVClassNode))
            {
                AddSemanticError(errs, string.Format(
                    "Cannot apply identifier selector to type '{0}', the type must be a class type",
                    currentType.Name), _pos, ErrorSeverity.Warning);
                return currentType;
            }

            // Check if the member exists
            _id = ((NVClassNode)currentType).FindMember(errs, _pos, _id.Name);
            if (_id == null)
            {
                return currentType;
            }

            _id.Type = ResolveNamedTypeNodes(errs, currentScope, _id.Type);
            _type = _id.Type;
            return _type;
        }

        public NVTypeNode ResolveNamedTypeNodes(ErrorHandler errs, Scope currentScope, NVTypeNode type)
        {
            if (type is NVNamedTypeNode)
            {
                NVIdNode newType = currentScope.Find(type.Name);
                if (newType != null)
                {
                    return (NVTypeNode)newType;
                }
                AddSemanticError(errs, string.Format("The type name '{0}' could not be found", type.Name), _pos,
                    ErrorSeverity.Warning);
            }

            return type;
        }

        public NVTypeNode GetParentType()
        {
            for (int i = 0; i < _nvReference.Designator.Selectors.Count; i++)
            {
                if (_nvReference.Designator.Selectors[i] == this)
                {
                    if (i == 0)
                    {
                        if (_nvReference.Designator.IdentNode == null)
                        {
                            return null;
                        }
                        return _nvReference.Designator.IdentNode.Type;
                    }
                    return _nvReference.Designator.Selectors[i - 1].Type;
                }
            }

            return null;
        }
    }
}