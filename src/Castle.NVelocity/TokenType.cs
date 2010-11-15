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

namespace Castle.NVelocity
{
    public enum TokenType
    {
        // General error token
        Error,
        
        // XML tokens
        XmlText, XmlAttributeText,
        XmlComment, XmlCommentStart, XmlCommentEnd,
        XmlTagName, XmlAttributeName, XmlAttributeMemberSelect,
        XmlTagStart, XmlTagEnd, XmlForwardSlash, XmlQuestionMark,
        XmlExclaimationMark, XmlEquals, XmlDoubleQuote, XmlSingleQuote, XmlCDataStart, XmlCDataEnd, XmlCDataSection,

        // NVelocity tokens
        NVSingleLineComment, NVMultilineCommentStart, NVMultilineCommentEnd, NVMultilineComment,
        NVText,
        NVDirectiveHash, NVDirectiveName, NVDirectiveLParen, NVDirectiveRParen, NVEscapeDirective,
        NVDollar,
        NVIdentifier, NVIntegerLiteral, /*NVFloatingPointLiteral,*/
        NVStringLiteral, NVEscape,
        NVDictionaryKey, NVDictionaryPercent, NVDictionaryLCurly, NVDictionaryRCurly, NVDictionaryEquals,
        NVReferenceLCurly, NVReferenceRCurly, NVReferenceSilent,
        NVLParen, NVRParen, NVLBrack, NVRBrack, NVLCurly, NVRCurly,
        NVDoubleQuote, NVSingleQuote,
        NVTrue, NVFalse, NVIn, NVWith,
        NVEq, NVLte, NVLt, NVGt, NVGte, NVEqEq, NVNeq,
        NVPlus, NVMinus, NVMul, NVDiv, NVMod,
        NVAnd, NVOr, NVNot,
        NVComma, NVDot, NVDoubleDot
    }
}