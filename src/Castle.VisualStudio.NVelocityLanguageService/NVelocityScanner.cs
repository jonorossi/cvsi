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
    using System.Collections.Generic;
    using Castle.NVelocity;
    using Microsoft.VisualStudio.Package;
    using TokenType = Castle.NVelocity.TokenType;

    public class NVelocityScanner : IScanner
    {
        private readonly Scanner scanner = new Scanner(new ErrorHandler());
        private readonly List<Stack<ScannerState>> lineState = new List<Stack<ScannerState>>();
        private bool isScanningLine = false;

        public NVelocityScanner()
        {
            scanner.Options.IsLineScanner = true;
            scanner.Options.SplitTextTokens = true;
            scanner.Options.EnableIntelliSenseTriggerTokens = true;

            // Insert the state for the initial line number
            Stack<ScannerState> initialState = new Stack<ScannerState>();
            initialState.Push(ScannerState.Default);
            lineState.Add(initialState);
        }

        public void SetSource(string source, int offset)
        {
            scanner.SetSource(source);
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int /*state*/ prevLineNumber)
        {
            if (!isScanningLine)
            {
                scanner.RestoreState(lineState[prevLineNumber]);
                isScanningLine = true;
            }

            Token token;
            try
            {
                token = scanner.GetToken();

                if (token == null && scanner.EOF)
                {
                    prevLineNumber = prevLineNumber + 1;
                    if (lineState.Count == prevLineNumber)
                        lineState.Add(scanner.RetrieveState());
                    else
                        lineState[prevLineNumber] = scanner.RetrieveState();

                    isScanningLine = false;
                    return false;
                }
            }
            catch (ScannerError)
            {
                isScanningLine = false;
                return false;
            }

            if (token == null || token.Type == TokenType.Error)
                return false;

            NVelocityTokenColor color = NVelocityTokenColor.XmlText;

            switch (token.Type)
            {
                // +==========================+
                // |     NVelocity Tokens     |
                // +==========================+
                case TokenType.NVText:
                case TokenType.NVEscapeDirective:
                case TokenType.NVComma:
                case TokenType.NVDoubleDot:
                    color = NVelocityTokenColor.NVText;
                    break;
                case TokenType.NVTrue:
                case TokenType.NVFalse:
                case TokenType.NVIn:
                case TokenType.NVWith:
                    color = NVelocityTokenColor.NVKeyword;
                    break;
                case TokenType.NVSingleLineComment:
                case TokenType.NVMultilineCommentStart:
                case TokenType.NVMultilineCommentEnd:
                case TokenType.NVMultilineComment:
                    color = NVelocityTokenColor.NVComment;
                    break;
                case TokenType.NVDollar:
                case TokenType.NVIdentifier:
                case TokenType.NVReferenceLCurly:
                case TokenType.NVReferenceRCurly:
                case TokenType.NVReferenceSilent:
                case TokenType.NVDot:
                    color = NVelocityTokenColor.NVIdentifier;
                    break;
                case TokenType.NVStringLiteral:
                case TokenType.NVDoubleQuote:
                case TokenType.NVSingleQuote:
                    color = NVelocityTokenColor.NVString;
                    break;
                case TokenType.NVIntegerLiteral:
                //case TokenType.NVFloatingPoint:
                    color = NVelocityTokenColor.NVNumber;
                    break;
                case TokenType.NVDirectiveHash:
                case TokenType.NVDirectiveName:
                case TokenType.NVDirectiveLParen:
                case TokenType.NVDirectiveRParen:
                    color = NVelocityTokenColor.NVDirective;
                    break;
                case TokenType.NVEq:
                case TokenType.NVLte:
                case TokenType.NVLt:
                case TokenType.NVGt:
                case TokenType.NVGte:
                case TokenType.NVEqEq:
                case TokenType.NVNeq:
                case TokenType.NVPlus:
                case TokenType.NVMinus:
                case TokenType.NVMul:
                case TokenType.NVDiv:
                case TokenType.NVMod:
                case TokenType.NVAnd:
                case TokenType.NVOr:
                case TokenType.NVNot:
                    color = NVelocityTokenColor.NVOperator;
                    break;
                case TokenType.NVLParen:
                case TokenType.NVRParen:
                case TokenType.NVLBrack:
                case TokenType.NVRBrack:
                case TokenType.NVLCurly:
                case TokenType.NVRCurly:
                    color = NVelocityTokenColor.NVBracket;
                    break;
                case TokenType.NVDictionaryPercent:
                case TokenType.NVDictionaryLCurly:
                case TokenType.NVDictionaryRCurly:
                    color = NVelocityTokenColor.NVDictionaryDelimiter;
                    break;
                case TokenType.NVDictionaryKey:
                    color = NVelocityTokenColor.NVDictionaryKey;
                    break;
                case TokenType.NVDictionaryEquals:
                    color = NVelocityTokenColor.NVDictionaryEquals;
                    break;

                // +====================+
                // |     XML Tokens     |
                // +====================+
                case TokenType.XmlText:
                    color = NVelocityTokenColor.XmlText;
                    break;
                case TokenType.XmlComment:
                case TokenType.XmlCommentStart:
                case TokenType.XmlCommentEnd:
                    color = NVelocityTokenColor.XmlComment;
                    break;
                case TokenType.XmlTagName:
                    color = NVelocityTokenColor.XmlTagName;
                    break;
                case TokenType.XmlAttributeName:
                    color = NVelocityTokenColor.XmlAttributeName;
                    break;
                case TokenType.XmlAttributeText:
                    color = NVelocityTokenColor.XmlAttributeValue;
                    break;
                case TokenType.XmlTagStart:
                case TokenType.XmlTagEnd:
                case TokenType.XmlCDataStart:
                case TokenType.XmlCDataEnd:
                    color = NVelocityTokenColor.XmlTagDelimiter;
                    break;
                case TokenType.XmlForwardSlash:
                case TokenType.XmlQuestionMark:
                case TokenType.XmlExclaimationMark:
                case TokenType.XmlEquals:
                case TokenType.XmlDoubleQuote:
                    color = NVelocityTokenColor.XmlOperator;
                    break;
                //case ???
                //    color = NVelocityTokenColor.XmlEntity;
                //    break;
                case TokenType.XmlCDataSection:
                    color = NVelocityTokenColor.XmlCDataSection;
                    break;
                //case ???
                //    color = NVelocityTokenColor.XmlProcessingInstruction;
                //    break;
            }

            tokenInfo.Color = (TokenColor)color;

            tokenInfo.StartIndex = token.Position.StartPos - 1;
            tokenInfo.EndIndex = token.Position.EndPos - 2;

            // Set the MemberSelect trigger on IntelliSense Member Completion characters
            switch (token.Type)
            {
                case TokenType.NVDollar:
                case TokenType.NVDot:
                case TokenType.NVDirectiveHash:
                case TokenType.NVDirectiveLParen:
                case TokenType.XmlTagStart:
                case TokenType.XmlAttributeMemberSelect:
                    tokenInfo.Trigger = TokenTriggers.MemberSelect;
                    break;

                case TokenType.NVLParen:
                case TokenType.NVRParen:
                    tokenInfo.Trigger = TokenTriggers.ParameterEnd | TokenTriggers.MatchBraces;
                    break;
                case TokenType.NVComma:
                    tokenInfo.Trigger = TokenTriggers.ParameterNext;
                    break;
            }

            return true;
        }
    }
}