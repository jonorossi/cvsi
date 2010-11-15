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

namespace Castle.NVelocity.Tests.ScannerTests
{
    using NUnit.Framework;

    [TestFixture]
    public class TokenPositionTestCase : ScannerTestBase
    {
        [Test]
        public void SingleLineTokenPositions()
        {
            _scanner.SetSource(
                "<name/>");

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.XmlTagName, "name", new Position(1, 2, 1, 6));
            AssertMatchToken(TokenType.XmlForwardSlash, new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(1, 7, 1, 8));

            AssertEOF();
        }

        [Test]
        public void MultilineTokenPositions()
        {
            _scanner.SetSource(
                "<name\n" +
                ">");

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.XmlTagName, "name", new Position(1, 2, 1, 6));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(2, 1, 2, 2));

            AssertEOF();
        }

        [Test]
        public void SplitTextTokenPositions()
        {
            _scanner.SetSource(
                "First Second !@#");

            _scanner.Options.SplitTextTokens = true;

            AssertMatchToken(TokenType.XmlText, "First", new Position(1, 1, 1, 6));
            AssertMatchToken(TokenType.XmlText, " ", new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.XmlText, "Second", new Position(1, 7, 1, 13));
            AssertMatchToken(TokenType.XmlText, " ", new Position(1, 13, 1, 14));
            AssertMatchToken(TokenType.XmlText, "!", new Position(1, 14, 1, 15));
            AssertMatchToken(TokenType.XmlText, "@", new Position(1, 15, 1, 16));
            AssertMatchToken(TokenType.XmlText, "#", new Position(1, 16, 1, 17));
        }

        [Test]
        public void ReferenceIdentifiers()
        {
            _scanner.SetSource(
                "text$first.Second.Third()");

            AssertMatchToken(TokenType.XmlText, "text", new Position(1, 1, 1, 5));
            AssertMatchToken(TokenType.NVDollar, new Position(1, 5, 1, 6));
            AssertMatchToken(TokenType.NVIdentifier, "first", new Position(1, 6, 1, 11));
            AssertMatchToken(TokenType.NVDot, new Position(1, 11, 1, 12));
            AssertMatchToken(TokenType.NVIdentifier, "Second", new Position(1, 12, 1, 18));
            AssertMatchToken(TokenType.NVDot, new Position(1, 18, 1, 19));
            AssertMatchToken(TokenType.NVIdentifier, "Third", new Position(1, 19, 1, 24));
            AssertMatchToken(TokenType.NVLParen, new Position(1, 24, 1, 25));
            AssertMatchToken(TokenType.NVRParen, new Position(1, 25, 1, 26));
        }

        [Test]
        public void ReferenceIdentifierInsideDirective()
        {
            _scanner.SetSource(
                "#if ($a)");

            AssertMatchToken(TokenType.NVDirectiveHash, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.NVDirectiveName, "if", new Position(1, 2, 1, 4));
            AssertMatchToken(TokenType.NVDirectiveLParen, new Position(1, 5, 1, 6));
            AssertMatchToken(TokenType.NVDollar, new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.NVIdentifier, "a", new Position(1, 7, 1, 8));
            AssertMatchToken(TokenType.NVDirectiveRParen, new Position(1, 8, 1, 9));
        }
    }
}