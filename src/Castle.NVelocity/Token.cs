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

namespace Castle.NVelocity
{
    public class Token
    {
        private TokenType _type;
        private string _image;
        private Position _position;

        public TokenType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                SetTokenImage();
            }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public void SetStartPosition(int startLine, int startPos)
        {
            if (_position == null)
                _position = new Position();

            _position.StartLine = startLine;
            _position.StartPos = startPos;
        }

        public void SetEndPosition(int endLine, int endPos)
        {
            if (_position == null)
                _position = new Position();

            _position.EndLine = endLine;
            _position.EndPos = endPos;
        }

        private void SetTokenImage()
        {
            switch (_type)
            {
                case TokenType.XmlTagStart: _image = "<"; break;
                case TokenType.XmlTagEnd: _image = ">"; break;
                case TokenType.XmlForwardSlash: _image = "/"; break;
                case TokenType.XmlQuestionMark: _image = "?"; break;
                case TokenType.XmlExclaimationMark: _image = "!"; break;
                case TokenType.XmlEquals: _image = "="; break;
                case TokenType.XmlDoubleQuote: _image = "\""; break;
                case TokenType.XmlCDataStart: _image = "<![CDATA["; break;
                case TokenType.XmlCDataEnd: _image = "]]>"; break;

                case TokenType.NVMultilineCommentStart: _image = "#*"; break;
                case TokenType.NVMultilineCommentEnd: _image = "*#"; break;
                case TokenType.NVDirectiveHash: _image = "#"; break;
                case TokenType.NVDollar: _image = "$"; break;
                case TokenType.NVReferenceLCurly: _image = "{"; break;
                case TokenType.NVReferenceRCurly: _image = "}"; break;
                case TokenType.NVReferenceSilent: _image = "!"; break;
                case TokenType.NVLParen: _image = "("; break;
                case TokenType.NVRParen: _image = ")"; break;
                case TokenType.NVLBrack: _image = "["; break;
                case TokenType.NVRBrack: _image = "]"; break;
                case TokenType.NVLCurly: _image = "{"; break;
                case TokenType.NVRCurly: _image = "}"; break;
                case TokenType.NVDictionaryPercent: _image = "%"; break;
                case TokenType.NVDictionaryLCurly: _image = "{"; break;
                case TokenType.NVDictionaryRCurly: _image = "}"; break;
                case TokenType.NVDictionaryEquals: _image = "="; break;
                case TokenType.NVDoubleQuote: _image = "\""; break;
                case TokenType.NVSingleQuote: _image = "'"; break;
                case TokenType.NVTrue: _image = "true"; break;
                case TokenType.NVFalse: _image = "false"; break;
                case TokenType.NVIn: _image = "in"; break;
                case TokenType.NVWith: _image = "with"; break;
                case TokenType.NVEq: _image = "="; break;
                case TokenType.NVLte: _image = "<="; break;
                case TokenType.NVLt: _image = "<"; break;
                case TokenType.NVGt: _image = ">"; break;
                case TokenType.NVGte: _image = ">="; break;
                case TokenType.NVEqEq: _image = "=="; break;
                case TokenType.NVNeq: _image = "!="; break;
                case TokenType.NVPlus: _image = "+"; break;
                case TokenType.NVMinus: _image = "-"; break;
                case TokenType.NVMul: _image = "*"; break;
                case TokenType.NVDiv: _image = "/"; break;
                case TokenType.NVMod: _image = "%"; break;
                case TokenType.NVAnd: _image = "&&"; break;
                case TokenType.NVOr: _image = "||"; break;
                case TokenType.NVNot: _image = "!"; break;
                case TokenType.NVComma: _image = ","; break;
                case TokenType.NVDot: _image = "."; break;
                case TokenType.NVDoubleDot: _image = ".."; break;
            }
        }
    }
}