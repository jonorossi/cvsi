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
    using Castle.NVelocity.Ast;
    using System.Collections.Generic;

    public class Parser
    {
        private readonly Scanner _scanner;
        private readonly ErrorHandler _errors;

        public Parser(Scanner scanner, ErrorHandler errors)
        {
            _scanner = scanner;
            _scanner.GetToken();

            _errors = errors;
        }

        private TokenType CurrentTokenType
        {
            get
            {
                if (_scanner.CurrentToken == null)
                {
                    return TokenType.Error;
                }
                return _scanner.CurrentToken.Type;
            }
        }

        private bool CurrentTokenIn(params TokenType[] tokenTypes)
        {
            if ( _scanner.CurrentToken == null)
            {
                return false;
            }

            TokenType currentTokenType = _scanner.CurrentToken.Type;
            foreach (TokenType tokenType in tokenTypes)
            {
                if (currentTokenType == tokenType)
                {
                    return true;
                }
            }

            return false;
        }

        private void MatchToken(TokenType expectedToken)
        {
            if (CurrentTokenIn(expectedToken))
            {
                _scanner.GetToken();
            }
            else
            {
                AddError(string.Format("Expected '{0}' but was '{1}'", expectedToken, CurrentTokenType));
            }
        }

        /// <summary>
        /// Returns a new instance of the current scanner token position.
        /// </summary>
        /// <returns>The current scanner token position.</returns>
        private Position GetCurrentTokenPosition()
        {
            // Don't just use the scanner's current position because the scanner allows
            // lookahead reading and will report that position.
            if (_scanner.CurrentToken != null)
            {
                Position pos = _scanner.CurrentToken.Position;
                return new Position(pos.StartLine, pos.StartPos, pos.EndLine, pos.EndPos);
            }
            return new Position(_scanner.CurrentPos);
        }

        public ErrorHandler Errors
        {
            get { return _errors; }
        }

        private void AddError(string description)
        {
            _errors.AddError(description, GetCurrentTokenPosition().End, ErrorSeverity.Error);
        }

        public TemplateNode ParseTemplate()
        {
            // Template -> XmlProlog { Content } EOF.

            TemplateNode templateNode = new TemplateNode();

            ParseXmlProlog();

            while (!_scanner.EOF)
            {
                List<AstNode> content = ParseContent();
                templateNode.AddRange(content);

                // This check ensures that if the parse isn't returning at nodes them the parser is stuck so terminate
                if (content.Count == 0)
                {
                    if (!_scanner.EOF)
                    {
                        AddError("The parser has been pre-emptively terminated because it appears as if the parser is stuck. [In ParseTemplate()]");
                    }
                    break;
                }
            }

            return templateNode;
        }

        private List<AstNode> ParseContent()
        {
            // Content -> XmlContent
            //         -> NVStatement.

            List<AstNode> content = new List<AstNode>();

            // Skip over NVSingleLineComment
            while (!_scanner.EOF && CurrentTokenType == TokenType.NVSingleLineComment)
            {
                _scanner.GetToken();
            }

            // Parse NVStatement and XmlContent
            if (CurrentTokenIn(TokenType.NVDirectiveHash, TokenType.NVDollar))
            {
                AstNode statement = ParseNVStatement();
                if (statement != null)
                {
                    content.Add(statement);
                }
            }
            else
            {
                content.AddRange(ParseXmlContent());
            }

            return content;
        }

        private void ParseXmlProlog()
        {
            // XmlProlog -> [ XmlDecl ] { XmlProcessingInstruction } [ XmlDoctTypeDecl ] { XmlProcessingInstruction }.

            //TODO Parse XML Prolog
        }

        //TODO: XmlDecl
        //TODO: XmlDeclVersionInfo
        //TODO: XmlDeclEncodingDecl
        //TODO: XmlDeclSDDecl
        //TODO: XmlProcessingInstruction
        //TODO: XmlDocTypeDecl

        private List<AstNode> ParseXmlContent()
        {
            // XmlContent -> { XmlElement | xmlText }.

            List<AstNode> content = new List<AstNode>();

            while (CurrentTokenIn(TokenType.XmlTagStart, TokenType.XmlText))
            {
                if (CurrentTokenType == TokenType.XmlTagStart)
                {
                    Token token = _scanner.PeekToken(1);

                    //TODO: does this need to be here because it stops the parsing of "<"
//                    // If no token follows then return the content list
//                    if (token == null)
//                    {
//                        return content;
//                    }

                    if (token != null && token.Type == TokenType.XmlForwardSlash)
                    {
                        // The '<' starts a end tag so exit the loop of the parent element body
                        // Return nodes created and do not process the closing tag
                        return content;
                    }

                    // Parse this element
                    content.Add(ParseXmlElement());
                }
                else if (CurrentTokenType == TokenType.XmlText)
                {
                    XmlTextNode xmlTextNode = new XmlTextNode(_scanner.CurrentToken.Image);
                    xmlTextNode.Position = GetCurrentTokenPosition();
                    content.Add(xmlTextNode);

                    _scanner.GetToken();
                }

                //TODO write some comments
                if (content.Count == 0 || (content.Count == 0 && _scanner.EOF))
                {
                    AddError("The parser has been pre-emptively terminated because it appears " +
                        "as if the parser is stuck. [In ParseXmlContent()]");
                    break;
                }
            }

            return content;
        }

        private XmlElement ParseXmlElement()
        {
            // XmlElement -> "<" XmlName { XmlAttribute | NVStatement } XmlRestElement.

            Position startPos = GetCurrentTokenPosition();

            MatchToken(TokenType.XmlTagStart);

            string elementName;
            if (CurrentTokenType == TokenType.XmlTagName)
            {
                elementName = _scanner.CurrentToken.Image;
                _scanner.GetToken();
            }
            else
            {
                AddError("Expected XML tag name.");
                XmlElement emptyXmlElement = new XmlElement("");
                emptyXmlElement.Position = startPos;

                // Consume TokenType.XmlAttributeMemberSelect tokens because these separate this partial element and the next node
                while (CurrentTokenType == TokenType.XmlAttributeMemberSelect)
                {
                    _scanner.GetToken();
                }

                return emptyXmlElement;
            }

            XmlElement xmlElement = new XmlElement(elementName);
            startPos.End = GetCurrentTokenPosition();
            xmlElement.Position = startPos;

            if (CurrentTokenIn(TokenType.NVDirectiveHash, TokenType.NVDollar))
            {
                AstNode statement = ParseNVStatement();
                if (statement != null)
                {
                    xmlElement.Attributes.Add(statement);
                }
            }

            // Consume TokenType.XmlAttributeMemberSelect tokens
			while (CurrentTokenType == TokenType.XmlAttributeMemberSelect)
			{
				// Do this before getting the next token because the next token may not belong to this XML element
				xmlElement.Position.End = GetCurrentTokenPosition().Start;

				_scanner.GetToken();
			}

            while (CurrentTokenType == TokenType.XmlAttributeName)
            {
                XmlAttribute xmlAttribute = ParseXmlAttribute();
                if (xmlAttribute != null)
                {
                    //TODO xmlAttribute.Position = GetCurrentTokenPosition();
                    xmlElement.Attributes.Add(xmlAttribute);
                }
                else if (!_scanner.EOF)
                {
                    //TODO write some comments
                    AddError("The parser has been pre-emptively terminated because it appears " +
                        "as if the parser is stuck. [In ParseXmlElement()]");
                    break;
                }

				xmlElement.Position.End = GetCurrentTokenPosition().Start;

                // Consume TokenType.XmlAttributeMemberSelect tokens
                while (CurrentTokenType == TokenType.XmlAttributeMemberSelect)
                {
					// Do this before getting the next token because the next token may not belong to this XML element
					xmlElement.Position.End = GetCurrentTokenPosition().Start;

					_scanner.GetToken();
                }
            }

			if (_scanner.EOF)
			{
				// Include the characters right to the end
				xmlElement.Position.End = GetCurrentTokenPosition();
			}

            ParseXmlRestElement(xmlElement);

            return xmlElement;
        }

        private void ParseXmlRestElement(XmlElement xmlElement)
        {
            // XmlRestElement -> "/" ">"
            //                -> ">" { Content } XmlEndTag.

            if (CurrentTokenType == TokenType.XmlForwardSlash)
            {
                xmlElement.IsSelfClosing = true;

                MatchToken(TokenType.XmlForwardSlash);
                if (CurrentTokenType == TokenType.XmlTagEnd)
                {
                    xmlElement.Position.End = GetCurrentTokenPosition();
                    xmlElement.IsComplete = true;

                    _scanner.GetToken();
                }
            }
            else if (CurrentTokenType == TokenType.XmlTagEnd)
            {
                MatchToken(TokenType.XmlTagEnd);

				xmlElement.Position.End = GetCurrentTokenPosition();

                while (!(CurrentTokenType == TokenType.XmlTagStart &&
                    _scanner.PeekToken(1) != null && _scanner.PeekToken(1).Type == TokenType.XmlForwardSlash))
                {
                    List<AstNode> content = ParseContent();
                    xmlElement.Content.AddRange(content);

                    // Set the parent property on the XmlElement
                    foreach (AstNode astNode in content)
                    {
                        if (astNode is XmlElement)
                        {
                            ((XmlElement)astNode).Parent = xmlElement;
                        }
                    }

                    //TODO write some comments
                    if (content.Count == 0 && !_scanner.EOF)
                    {
                        AddError("The parser has been pre-emptively terminated because it appears " +
                            "as if the parser is stuck. [In ParseXmlRestElement()]");
                        break;
                    }

                    if (_scanner.EOF)
                    {
                        AddError(string.Format("Expected closing tag for opening tag '{0}'.", xmlElement.Name));
                        break;
                    }
                }

                ParseXmlEndTag(xmlElement);
            }
			else
			{
				AddError("Expected end of XML element");
			}
        }

        private void ParseXmlEndTag(XmlElement xmlElement)
        {
            // XmlEndTag -> "<" "/" XmlName ">".

            if (_scanner.EOF)
            {
                return;
            }

            MatchToken(TokenType.XmlTagStart);
            MatchToken(TokenType.XmlForwardSlash);

            //TODO use ParseXmlName instead of just getting the image
            if (CurrentTokenType == TokenType.XmlTagName)
            {
                if (xmlElement.Name != _scanner.CurrentToken.Image)
                {
                    AddError("Mismatched element start and end tags");
                }
                _scanner.GetToken();
            }

            if (CurrentTokenType == TokenType.XmlTagEnd)
            {
                xmlElement.Position.End = GetCurrentTokenPosition();
                xmlElement.EndTagPosition = GetCurrentTokenPosition();
                xmlElement.IsComplete = true;
                _scanner.GetToken();
            }
            else
            {
                AddError("Expected '>'");
            }
        }

        private XmlAttribute ParseXmlAttribute()
        {
            // XmlAttribute -> XmlName "=" ( "\"" { xmlAttributeText | NVStatement } "\""
            //                             | "'"  { xmlAttributeText | NVStatement } "'" ).

            XmlAttribute xmlAttribute;

            //TODO: ParseXmlName instead of matching an XmlAttributeName
            if (CurrentTokenType == TokenType.XmlAttributeName)
            {
                xmlAttribute = new XmlAttribute(_scanner.CurrentToken.Image);
                _scanner.GetToken();
            }
            else
            {
                AddError("Expected attribute name");
                return null;
            }

            MatchToken(TokenType.XmlEquals);

            bool doubleQuotes;
            if (_scanner.CurrentToken != null && _scanner.CurrentToken.Type == TokenType.XmlDoubleQuote)
            {
                _scanner.GetToken();
                doubleQuotes = true;
            }
            else if (_scanner.CurrentToken != null && _scanner.CurrentToken.Type == TokenType.XmlSingleQuote)
            {
                _scanner.GetToken();
                doubleQuotes = false;
            }
            else
            {
                AddError("Expected quotes around attribute value.");
                return null;
            }

            while ((CurrentTokenType != TokenType.XmlDoubleQuote && doubleQuotes) ||
                (CurrentTokenType != TokenType.XmlSingleQuote && !doubleQuotes))
            {
                AstNode astNode;

                if (CurrentTokenType == TokenType.XmlAttributeText)
                {
                    astNode = new XmlTextNode(_scanner.CurrentToken.Image);
                    _scanner.GetToken();
                }
                else if (CurrentTokenIn(TokenType.NVDirectiveHash, TokenType.NVDollar))
                {
                    astNode = ParseNVStatement();
                }
                else
                {
                    AddError("Expected XML attribute value or NVelocity statement.");
                    break;
                }

                xmlAttribute.Content.Add(astNode);
            }
            //TODO: else if(CurrentTokenType == TokenType.XmlSingleQuote)

            if (doubleQuotes)
            {
                MatchToken(TokenType.XmlDoubleQuote);
            }
            else
            {
                MatchToken(TokenType.XmlSingleQuote);
            }

            return xmlAttribute;
        }

        //TODO: ParseXmlName

        private AstNode ParseNVStatement()
        {
            // NVStatement -> NVDirective
            //             -> NVReference.

            if (CurrentTokenType == TokenType.NVDirectiveHash)
            {
                return ParseNVDirective();
            }

            if (CurrentTokenType == TokenType.NVDollar)
            {
                return ParseNVReference();
            }

            AddError("Expected directive or reference");
            return null;
        }

        private NVDirective ParseNVDirective()
        {
            // NVDirective -> "#" NVRestDirective.

            Position startPos = new Position(GetCurrentTokenPosition().Start);

            MatchToken(TokenType.NVDirectiveHash);

            NVDirective nvDirective = ParseNVRestDirective(startPos);

            return nvDirective;
        }

        private NVDirective ParseNVRestDirective(Position startPos)
        {
            // NVRestDirective -> "if" "(" NVExpression ")" Content { NVElseIfStatement } [ NVElseStatement ] NVEnd
            //                 -> "set" "(" NVReference "=" NVExpression ")"
            //                 -> "stop"
            //                 -> "foreach" "(" "$" nvIdentifier "in" NVExpression ")" Content NVEnd.
            //                 -> nvDirectiveName [ "(" { nvIdentifier | NVExpression } ")" [ Content NVEnd ] ].

            NVDirective nvDirective;

            if (_scanner.CurrentToken != null && _scanner.CurrentToken.Type == TokenType.NVDirectiveName)
            {
                if (_scanner.CurrentToken.Image == "foreach")
                {
                    nvDirective = new NVForeachDirective();
                }
                else
                {
                    nvDirective = new NVDirective(_scanner.CurrentToken.Image);
                }
                _scanner.GetToken();
            }
            else
            {
                AddError("Expected directive name");
                nvDirective = new NVDirective("");
                nvDirective.Position = new Position(startPos, GetCurrentTokenPosition());
                return nvDirective;
            }

            // Match directive parameters
            if (CurrentTokenType == TokenType.NVDirectiveLParen)
            {
                _scanner.GetToken();
                if (nvDirective is NVForeachDirective)
                {
                    MatchToken(TokenType.NVDollar);
                    if (_scanner.CurrentToken != null && _scanner.CurrentToken.Type == TokenType.NVIdentifier)
                    {
                        ((NVForeachDirective)nvDirective).Iterator = _scanner.CurrentToken.Image;
                        _scanner.GetToken();
                    }
                    else
                    {
                        AddError("Foreach variable declaration expected");
                    }

                    MatchToken(TokenType.NVIn);

                    ((NVForeachDirective)nvDirective).Collection = ParseNVExpression();
                }
                else
                {
                    while (!_scanner.EOF && !CurrentTokenIn(TokenType.NVDirectiveRParen, TokenType.NVDirectiveHash))
                    {
                        _scanner.GetToken();

                        // If the tokens are errors break out
                        if (CurrentTokenType == TokenType.Error)
                        {
                            AddError("Unable to parse all directive contents.");
                            break;
                        }
                    }

                    // If a new directive starts in the middle of an unfinished directive params then stop parsing this directive
                    if (CurrentTokenType != TokenType.NVDirectiveRParen)
                    {
                        AddError("Incomplete directive.");

                        nvDirective.Position = new Position(startPos, GetCurrentTokenPosition());
                        return nvDirective;
                    }
                }
                MatchToken(TokenType.NVDirectiveRParen);
            }

            // Match directive content and #end if required
            if (nvDirective is NVForeachDirective)
            {
                while (!(CurrentTokenType == TokenType.NVDirectiveHash &&
                    _scanner.PeekToken(1) != null &&
                    _scanner.PeekToken(1).Type == TokenType.NVDirectiveName &&
                    _scanner.PeekToken(1).Image == "end"))
                {
                    List<AstNode> content = ParseContent();
                    ((NVForeachDirective)nvDirective).Content.AddRange(content);

                    // If this is the end of the file then return what has been build
                    if (_scanner.EOF)
                    {
                        AddError("Expected #end directive.");
                        nvDirective.Position = new Position(startPos, _scanner.CurrentPos);
                        return nvDirective;
                    }

                    // Break out of the loop if no content was found because the directive is incomplete
                    if (content.Count == 0)
                    {
                        AddError("The parser has been pre-emptively terminated because it appears " +
                            "as if the parser is stuck. [In ParseNVForeachDirective()]");
                        break;
                    }
                }

                // Match #end
                MatchToken(TokenType.NVDirectiveHash);
                if (CurrentTokenType == TokenType.NVDirectiveName)
                {
                    if (_scanner.CurrentToken.Image != "end")
                    {
                        AddError("Expected #end directive.");
                    }

                    nvDirective.Position = new Position(startPos, _scanner.CurrentPos);
                    _scanner.GetToken();
                }
                else
                {
                    _scanner.GetToken();
                    nvDirective.Position = new Position(startPos, _scanner.CurrentPos);
                }
            }
            else
            {
                nvDirective.Position = new Position(startPos, GetCurrentTokenPosition());
            }

            return nvDirective;
        }

        //TODO: NVElseIfStatement
        //TODO: NVElseStatement
        //TODO: NVEnd

        private NVReference ParseNVReference()
        {
            // NVReference -> "$" [ "!" ] ( NVDesignator | "{" NVDesignator "}" ).

            Position startPosition = new Position(_scanner.CurrentPos.Start);

            MatchToken(TokenType.NVDollar);
            
            if (CurrentTokenType == TokenType.NVReferenceSilent)
            {
                _scanner.GetToken();
            }

            //TODO: Match '{' properly
            if (CurrentTokenType == TokenType.NVReferenceLCurly)
            {
                _scanner.GetToken();
            }

            NVReference nvReference = ParseNVDesignator(startPosition);

            //TODO: Match '}' properly
            if (CurrentTokenType == TokenType.NVReferenceRCurly)
            {
                _scanner.GetToken();
            }

            return nvReference;
        }

        private NVReference ParseNVDesignator(Position startPosition)
        {
            // NVDesignator -> nvIdentifer { "." nvIdentifier [ NVActualParams ] }.

            NVReference nvReference;
            if (CurrentTokenType == TokenType.NVIdentifier)
            {
                nvReference = new NVReference(new NVDesignator(_scanner.CurrentToken.Image));
                nvReference.Position = new Position(startPosition, _scanner.CurrentPos);
                _scanner.GetToken();
            }
            else
            {
                AddError("Expected reference identifier");
                nvReference = new NVReference(new NVDesignator(""));
                nvReference.Position = new Position(startPosition, _scanner.CurrentPos);
                return nvReference;
            }

            while (CurrentTokenType == TokenType.NVDot)
            {
                Position startSelectorPosition = new Position(_scanner.CurrentPos.Start);
                Position dotPosition = new Position(_scanner.CurrentPos);
                _scanner.GetToken();

                if (CurrentTokenType == TokenType.NVIdentifier)
                {
                    Position endSelectorPosition = new Position(_scanner.CurrentPos.End);

                    NVSelector selector = new NVSelector(new NVMethodNode(
                        _scanner.CurrentToken.Image), nvReference);
                    _scanner.GetToken();

                    if (CurrentTokenType == TokenType.NVLParen)
                    {
                        selector.Actuals = ParseNVActualParams();
                        endSelectorPosition = new Position(_scanner.PreviousPos.Start);
                    }

                    selector.Position = new Position(startSelectorPosition, endSelectorPosition);

                    nvReference.Designator.Selectors.Add(selector);

                    nvReference.Position.End = new Position(endSelectorPosition);
                }
                else
                {
                    AddError("Expected identifier");
                    
                    NVSelector selector = new NVSelector(new NVMethodNode("", null), nvReference);
                    selector.Position = new Position(dotPosition);
                    nvReference.Designator.Selectors.Add(selector);

                    nvReference.Position.End = new Position(selector.Position.End);

                    return nvReference;
                }
            }

            return nvReference;
        }

        private List<NVExpression> ParseNVActualParams()
        {
            // NVActualParams -> "(" [ NVExpression { "," NVExpression } ] ")".

            List<NVExpression> actuals = new List<NVExpression>();

            MatchToken(TokenType.NVLParen);
            if (CurrentTokenType != TokenType.NVRParen)
            {
                NVExpression expr = ParseNVExpression();
                if (expr != null)
                {
                    actuals.Add(expr);
                }

                while (CurrentTokenType == TokenType.NVComma)
                {
                    _scanner.GetToken();
                    expr = ParseNVExpression();
                    if (expr != null)
                    {
                        actuals.Add(expr);
                    }
                }
            }
            MatchToken(TokenType.NVRParen);

            return actuals;
        }

        private NVExpression ParseNVExpression()
        {
            // NVExpression -> NVAndExpression { ( "||" | "or" ) NVAndExpression }.

            NVExpression expr = ParseNVAndExpression();

            while (CurrentTokenType == TokenType.NVOr)
            {
                _scanner.GetToken();
                expr = new NVBinaryExpression(Operator.Or, expr, ParseNVAndExpression());
                expr.Position = new Position((NVBinaryExpression)expr);
            }
            
            return expr;
        }

        private NVExpression ParseNVAndExpression()
        {
            // NVAndExpression -> NVRelExpression { ( "&&" | "and" ) NVRelExpression }.

            NVExpression expr = ParseNVRelExpression();

            while (CurrentTokenType == TokenType.NVAnd)
            {
                _scanner.GetToken();
                expr = new NVBinaryExpression(Operator.And, expr, ParseNVRelExpression());
                expr.Position = new Position((NVBinaryExpression)expr);
            }

            return expr;
        }

        private NVExpression ParseNVRelExpression()
        {
            // NVRelExpression -> NVSimpleExpression [ NVRelOp NVSimpleExpression ].

            NVExpression expr = ParseNVSimpleExpression();

            if (CurrentTokenIn(TokenType.NVLte, TokenType.NVLt, TokenType.NVGt, TokenType.NVGte,
                TokenType.NVEqEq, TokenType.NVNeq))
            {
                expr = new NVBinaryExpression(ParseNVRelOp(), expr, ParseNVSimpleExpression());
                expr.Position = new Position((NVBinaryExpression)expr);
            }

            return expr;
        }

        private NVExpression ParseNVSimpleExpression()
        {
            // NVSimpleExpression -> NVTerm { NVAddOp NVTerm }.

            NVExpression expr = ParseNVTerm();

            while (CurrentTokenIn(TokenType.NVPlus, TokenType.NVMinus))
            {
                expr = new NVBinaryExpression(ParseNVAddOp(), expr, ParseNVTerm());
                expr.Position = new Position((NVBinaryExpression)expr);
            }

            return expr;
        }

        private NVExpression ParseNVTerm()
        {
            // NVTerm -> NVFactor { NVMulOp NVFactor }.

            NVExpression expr = ParseNVFactor();

            while (CurrentTokenIn(TokenType.NVMul, TokenType.NVDiv, TokenType.NVMod))
            {
                expr = new NVBinaryExpression(ParseNVMulOp(), expr, ParseNVFactor());
                expr.Position = new Position((NVBinaryExpression)expr);
            }

            return expr;
        }

        private NVExpression ParseNVFactor()
        {
            // NVFactor -> NVPrimary
            //          -> ( "+" | "-" | "!" | "not" ) NVFactor.

            NVExpression expr;

            if (CurrentTokenIn(TokenType.NVPlus, TokenType.NVMinus, TokenType.NVNot))
            {
                //TODO: op = Operator...
                _scanner.GetToken();

                //TODO: expr = new UnaryExpression(op, ParseNVFactor);
                expr = ParseNVFactor();
            }
            else
            {
                expr = ParseNVPrimary();
            }

            return expr;
        }

        private NVExpression ParseNVPrimary()
        {
            // NVPrimary -> NVReference
            //           -> "(" NVExpression ")"
            //           -> "true"
            //           -> "false"
            //           -> nvIntegerLiteral
            //           -> "'" nvStringLiteral "'"
            //           -> "\"" NVStringLiteralOrDictionary "\""
            //           -> "[" [ NVExpression NVRangeOrArray ] "]".

            NVExpression expr = null;

            if (CurrentTokenType == TokenType.NVDollar)
            {
                //TODO: Change to ParseNVDesignatorExpression
                ParseNVReference();

                expr = new NVBoolExpression(false);
                expr.Position = new Position(GetCurrentTokenPosition());
            }
            else if (CurrentTokenType == TokenType.NVLParen)
            {
                _scanner.GetToken();
                
                expr = ParseNVExpression();

                MatchToken(TokenType.NVRParen);
            }
            else if (CurrentTokenType == TokenType.NVTrue)
            {
                expr = new NVBoolExpression(true);
                expr.Position = new Position(GetCurrentTokenPosition());
                _scanner.GetToken();
            }
            else if (CurrentTokenType == TokenType.NVFalse)
            {
                expr = new NVBoolExpression(false);
                expr.Position = new Position(GetCurrentTokenPosition());
                _scanner.GetToken();
            }
            else if (CurrentTokenType == TokenType.NVIntegerLiteral)
            {
                expr = new NVNumExpression(int.Parse(_scanner.CurrentToken.Image));
                expr.Position = new Position(GetCurrentTokenPosition());
                _scanner.GetToken();
            }
            else if (CurrentTokenType == TokenType.NVSingleQuote)
            {
                _scanner.GetToken();

                if (CurrentTokenType == TokenType.NVStringLiteral)
                {
                    expr = new NVStringExpression(_scanner.CurrentToken.Image);
                    expr.Position = new Position(GetCurrentTokenPosition());
                    _scanner.GetToken();
                }

                if (CurrentTokenType == TokenType.NVSingleQuote)
                {
                    _scanner.GetToken();
                }
                else
                {
                    AddError("Expected single quote for closing string literal");
                }
            }
            else if (CurrentTokenType == TokenType.NVDoubleQuote)
            {
                _scanner.GetToken();

                expr = ParseNVStringLiteralOrDictionary();

                MatchToken(TokenType.NVDoubleQuote);
            }
            else if (CurrentTokenType == TokenType.NVLBrack)
            {
                _scanner.GetToken();

                if (CurrentTokenType != TokenType.NVRBrack)
                {
                    ParseNVExpression();
                    expr = ParseNVRangeOrArray();
                }
                else
                {
                    //TODO: replace with an empty NVArrayExpression
                    expr = new NVBoolExpression(false);
                    expr.Position = new Position(GetCurrentTokenPosition());
                }

                MatchToken(TokenType.NVRBrack);
            }
            else
            {
                AddError(string.Format("Expected expression, was '{0}'", CurrentTokenType));
            }

            return expr;
        }

        private NVExpression ParseNVStringLiteralOrDictionary()
        {
            // NVStringLiteralOrDictionary -> { nvText | NVReference }
            //                             -> "%" "{" [ NVDictionaryItem { "," NVDictionaryItem } ] "}".

            NVExpression expr = null;

            if (CurrentTokenType == TokenType.NVDictionaryPercent)
            {
                _scanner.GetToken();

                MatchToken(TokenType.NVDictionaryLCurly);

                if (CurrentTokenType == TokenType.NVDictionaryKey)
                {
                    ParseNVDictionaryItem();

                    while (CurrentTokenType == TokenType.NVComma)
                    {
                        _scanner.GetToken();

                        ParseNVDictionaryItem();
                    }
                }

                MatchToken(TokenType.NVDictionaryRCurly);

                //TODO: Change to DictionaryExpression, this is just a 
                expr = new NVBoolExpression(false);
                expr.Position = new Position(GetCurrentTokenPosition());
            }
            else
            {
                //TODO: { nvText | NVReference }
                while (CurrentTokenType != TokenType.NVDoubleQuote)
                {
                    if (CurrentTokenType == TokenType.NVStringLiteral)
                    {
                        expr = new NVStringExpression(_scanner.CurrentToken.Image);
                        expr.Position = new Position(GetCurrentTokenPosition());
                    }
                    //TODO make only work in intellisense mode
                    else if (CurrentTokenType == TokenType.Error)
                    {
                        //TODO: check if this is causing CVSI to lock up
                        return new NVStringExpression("");
                    }
                    else
                    {
                        AddError(string.Format("Unexpected token type '{0}'", CurrentTokenType));
                        break;
                    }
                    _scanner.GetToken();
                }
            }

            return expr;
        }

        private void ParseNVDictionaryItem()
        {
            // NVDictionaryItem	-> ( nvIdentifier | NVExpression ) "=" ( NVExpression | NVDictionary ).

            if (CurrentTokenType == TokenType.NVDictionaryKey)
            {
                _scanner.GetToken();
            }
            else
            {
                ParseNVExpression();
            }

            MatchToken(TokenType.NVDictionaryEquals);

            //TODO
            ParseNVExpression();
        }
        
        private NVExpression ParseNVRangeOrArray()
        {
            // NVRangeOrArray -> ".." NVExpression
	        //                -> { "," NVExpression }.

            NVExpression expr;

            if (CurrentTokenType == TokenType.NVDoubleDot)
            {
                _scanner.GetToken();

                ParseNVExpression();

                //TODO: replace with NVRangeExpression
                expr = new NVBoolExpression(false);
                expr.Position = new Position(GetCurrentTokenPosition());
            }
            else
            {
                while (CurrentTokenType != TokenType.NVRBrack)
                {
                    if (CurrentTokenType == TokenType.NVComma)
                    {
                        _scanner.GetToken();
                    }
                    else
                    {
                        AddError("Expected ','");
                        break;
                    }

                    ParseNVExpression();
                }

                //TODO: replace with NVArrayExpression
                expr = new NVBoolExpression(false);
                expr.Position = new Position(GetCurrentTokenPosition());
            }

            return expr;
        }

        private Operator ParseNVRelOp()
        {
            // NVRelOp -> "<=" | "le" | "<" | "lt" | ">" | "gt" | ">=" | "ge" | "==" | "eq" | "!=" | "ne".

            Operator op = default(Operator);

            switch (CurrentTokenType)
            {
                case TokenType.NVLte:
                    op = Operator.Lte;
                    _scanner.GetToken();
                    break;
                case TokenType.NVLt:
                    op = Operator.Lt;
                    _scanner.GetToken();
                    break;
                case TokenType.NVGt:
                    op = Operator.Gt;
                    _scanner.GetToken();
                    break;
                case TokenType.NVGte:
                    op = Operator.Gte;
                    _scanner.GetToken();
                    break;
                case TokenType.NVEqEq:
                    op = Operator.EqEq;
                    _scanner.GetToken();
                    break;
                case TokenType.NVNeq:
                    op = Operator.Neq;
                    _scanner.GetToken();
                    break;
                default:
                    AddError("Expected relational operator");
                    break;
            }

            return op;
        }

        private Operator ParseNVAddOp()
        {
            // NVAddOp -> "+" | "-".

            Operator op = default(Operator);

            switch (CurrentTokenType)
            {
                case TokenType.NVPlus:
                    op = Operator.Plus;
                    _scanner.GetToken();
                    break;
                case TokenType.NVMinus:
                    op = Operator.Minus;
                    _scanner.GetToken();
                    break;
                default:
                    AddError("Expected addition operator");
                    break;
            }

            return op;
        }

        private Operator ParseNVMulOp()
        {
            // NVMulOp -> "*" | "/" | "%".

            Operator op = default(Operator);

            switch (CurrentTokenType)
            {
                case TokenType.NVMul:
                    op = Operator.Mul;
                    _scanner.GetToken();
                    break;
                case TokenType.NVDiv:
                    op = Operator.Div;
                    _scanner.GetToken();
                    break;
                case TokenType.NVMod:
                    op = Operator.Mod;
                    _scanner.GetToken();
                    break;
                default:
                    AddError("Expected multiplication operator");
                    break;
            }

            return op;
        }
    }
}