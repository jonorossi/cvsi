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

namespace Castle.NVelocity.Tests.ScannerTests
{
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityDirectiveTestCase : ScannerTestBase
    {
        [Test]
        public void IfDirectiveInsideXmlText()
        {
            _scanner.SetSource(
                "text #if text");

            AssertMatchToken(TokenType.XmlText, "text ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesInsideXmlText()
        {
            _scanner.SetSource(
                "text #{if} text");

            AssertMatchToken(TokenType.XmlText, "text ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesWithXmlTextTouchingDirective()
        {
            _scanner.SetSource(
                "text#{if}text");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.XmlText, "text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithOnlyLCurly()
        {
            _scanner.SetSource(
                "text#{if");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");

            Assert.AreEqual(1, _scanner.Errors.Count);
            Assert.AreEqual("Scanner: Expected '}' for closing directive name", _scanner.Errors[0].Description);
        }

        [Test]
        public void IfDirectiveFollowedByParens()
        {
            _scanner.SetSource(
                "text#if()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveWithBracesFollowedByParens()
        {
            _scanner.SetSource(
                "text#{if}()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveFollowedBySpaceAndParens()
        {
            _scanner.SetSource(
                "text#if ()");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void EndDirectiveFollowedByXmlText()
        {
            _scanner.SetSource(
                "text#end text");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");
            AssertMatchToken(TokenType.XmlText, " text");

            AssertEOF();
        }

        [Test]
        public void IfDirectiveInsideXmlTag()
        {
            _scanner.SetSource(
                "<div #if() />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void IfDirectiveInsideXmlAttributeValue()
        {
            _scanner.SetSource(
                "<div class=\"#if() text #end\" />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlAttributeText, " text ");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");

            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void DirectiveParamsAreOnlyScannedForOnTheSameLineAsTheDirective()
        {
            _scanner.SetSource(
                "#foreach ($item in $items)\n" +
                "  #nodata\n" +
                "    (none available)\n" +
                "#end");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "foreach");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "item");
            AssertMatchToken(TokenType.NVIn);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "items");
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlText, "\n  ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "nodata");

            AssertMatchToken(TokenType.XmlText, "\n    (none available)\n");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");
        }

        [Test]
        public void DirectivesWithoutParamsDoNotConsumeFollowingWhitespaceWhenFollowedByLParenOnNextLine()
        {
            _scanner.SetSource(
                "#stop   \n" +
                "(text)");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "stop");
            AssertMatchToken(TokenType.XmlText, "   \n(text)");
        }
    }
}