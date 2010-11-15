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

namespace Castle.VisualStudio.NVelocityLanguageService
{
    public enum IntelliSenseIcon
    {
        Class = 0,
        Macro = 54,
        Method = 72,
        Property = 102,
        Variable = 138,
        Error = 186,
        Warning = 216,
        Information = 207,
        XmlElement = 210,
		XmlAttribute = 102
    }
}

//public enum Accessibility
//{
//    Public = 0,
//    Internal = 1,
//    Friend = 2,
//    Protected = 3,
//    Private = 4,
//    Shortcut = 5,
//}
//
//public enum Element
//{
//    Class = 0,
//    Constant = 1,
//    Delegate = 2,
//    Enum = 3,
//    Enummember = 4,
//    Event = 5,
//    Exception = 6,
//    Field = 7,
//    Interface = 8,
//    Macro = 9,
//    Map = 10,
//    Mapitem = 11,
//    Method = 12,
//    Overload = 13,
//    Module = 14,
//    Namespace = 15,
//    Operator = 16,
//    Property = 17,
//    Struct = 18,
//    Template = 19,
//    Typedef = 20,
//    Type = 21,
//    Union = 22,
//    Variable = 23,
//    Valuetype = 24,
//    Intrinsic = 25,
//}
//
//public static int GetIndexFor(Accessibility accessibility, Element element)
//{
//    return (int)accessibility + (int)element * 6;
//}