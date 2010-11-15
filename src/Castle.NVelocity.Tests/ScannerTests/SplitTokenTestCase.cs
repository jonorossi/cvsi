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
    public class SplitTokenTestCase : ScannerTestBase
    {
        [Test]
        public void ReturnsWordsAsSeperateTokens()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "First Second Third");

            AssertMatchToken(TokenType.XmlText, "First");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "Second");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "Third");
        }

        [Test]
        public void ReturnsMultipleSpacesAsASingleToken()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "First   Second");

            AssertMatchToken(TokenType.XmlText, "First");
            AssertMatchToken(TokenType.XmlText, "   ");
            AssertMatchToken(TokenType.XmlText, "Second");
        }

        [Test]
        public void ReturnsPunctionationCharsSeperately()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "First! Second ? !@#");

            AssertMatchToken(TokenType.XmlText, "First");
            AssertMatchToken(TokenType.XmlText, "!");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "Second");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "?");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "!");
            AssertMatchToken(TokenType.XmlText, "@");
            AssertMatchToken(TokenType.XmlText, "#");
        }

        [Test]
        public void ReferencesAndDirectivesAreScanned()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "text $var #directive text #$%");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "directive");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.XmlText, " ");
            AssertMatchToken(TokenType.XmlText, "#");
            AssertMatchToken(TokenType.XmlText, "$");
            AssertMatchToken(TokenType.XmlText, "%");
        }

        [Test]
        public void ReturnsWordsAsSeperateTokensWithinXmlAttributeValue()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "<div class=\"someClass anotherClass\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "someClass");
            AssertMatchToken(TokenType.XmlAttributeText, " ");
            AssertMatchToken(TokenType.XmlAttributeText, "anotherClass");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void ReturnsWordsAsSeperateTokensWithinCDataSection()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "<![CDATA[First Second Third]]>");

            AssertMatchToken(TokenType.XmlCDataStart, new Position(1, 1, 1, 10));
            AssertMatchToken(TokenType.XmlCDataSection, "First", new Position(1, 10, 1, 15));
            AssertMatchToken(TokenType.XmlCDataSection, " ", new Position(1, 15, 1, 16));
            AssertMatchToken(TokenType.XmlCDataSection, "Second", new Position(1, 16, 1, 22));
            AssertMatchToken(TokenType.XmlCDataSection, " ", new Position(1, 22, 1, 23));
            AssertMatchToken(TokenType.XmlCDataSection, "Third", new Position(1, 23, 1, 28));
            AssertMatchToken(TokenType.XmlCDataEnd, new Position(1, 28, 1, 31));
        }

        [Test]
        public void ReturnsWordsAsSeperateTokensWithinScriptElement()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "<script>This is JavaScript</script>");

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.XmlTagName, "script", new Position(1, 2, 1, 8));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(1, 8, 1, 9));
            
            AssertMatchToken(TokenType.XmlText, "This", new Position(1, 9, 1, 13));
            AssertMatchToken(TokenType.XmlText, " ", new Position(1, 13, 1, 14));
            AssertMatchToken(TokenType.XmlText, "is", new Position(1, 14, 1, 16));
            AssertMatchToken(TokenType.XmlText, " ", new Position(1, 16, 1, 17));
            AssertMatchToken(TokenType.XmlText, "JavaScript", new Position(1, 17, 1, 27));

            AssertMatchToken(TokenType.XmlTagStart, new Position(1, 27, 1, 28));
            AssertMatchToken(TokenType.XmlForwardSlash, new Position(1, 28, 1, 29));
            AssertMatchToken(TokenType.XmlTagName, "script", new Position(1, 29, 1, 35));
            AssertMatchToken(TokenType.XmlTagEnd, new Position(1, 35, 1, 36));
        }

        [Test]
        public void ReturnsWordsAsSeperateTokensWithinNVDoubleQuote()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "$a.B(\"First $second Third\")");

            AssertMatchToken(TokenType.NVDollar, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.NVIdentifier, "a", new Position(1, 2, 1, 3));
            AssertMatchToken(TokenType.NVDot, new Position(1, 3, 1, 4));
            AssertMatchToken(TokenType.NVIdentifier, "B", new Position(1, 4, 1, 5));
            AssertMatchToken(TokenType.NVLParen, new Position(1, 5, 1, 6));
            AssertMatchToken(TokenType.NVDoubleQuote, new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.NVStringLiteral, "First", new Position(1, 7, 1, 12));
            AssertMatchToken(TokenType.NVStringLiteral, " ", new Position(1, 12, 1, 13));
            AssertMatchToken(TokenType.NVDollar, new Position(1, 13, 1, 14));
            AssertMatchToken(TokenType.NVIdentifier, "second", new Position(1, 14, 1, 20));
            AssertMatchToken(TokenType.NVStringLiteral, " ", new Position(1, 20, 1, 21));
            AssertMatchToken(TokenType.NVStringLiteral, "Third", new Position(1, 21, 1, 26));
            AssertMatchToken(TokenType.NVDoubleQuote, new Position(1, 26, 1, 27));
            AssertMatchToken(TokenType.NVRParen, new Position(1, 27, 1, 28));
        }

        [Test]
        public void ReturnsWordsAsSeperateTokensWithinNVSingleQuote()
        {
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "$a.B('First Second Third')");

            AssertMatchToken(TokenType.NVDollar, new Position(1, 1, 1, 2));
            AssertMatchToken(TokenType.NVIdentifier, "a", new Position(1, 2, 1, 3));
            AssertMatchToken(TokenType.NVDot, new Position(1, 3, 1, 4));
            AssertMatchToken(TokenType.NVIdentifier, "B", new Position(1, 4, 1, 5));
            AssertMatchToken(TokenType.NVLParen, new Position(1, 5, 1, 6));
            AssertMatchToken(TokenType.NVSingleQuote, new Position(1, 6, 1, 7));
            AssertMatchToken(TokenType.NVStringLiteral, "First", new Position(1, 7, 1, 12));
            AssertMatchToken(TokenType.NVStringLiteral, " ", new Position(1, 12, 1, 13));
            AssertMatchToken(TokenType.NVStringLiteral, "Second", new Position(1, 13, 1, 19));
            AssertMatchToken(TokenType.NVStringLiteral, " ", new Position(1, 19, 1, 20));
            AssertMatchToken(TokenType.NVStringLiteral, "Third", new Position(1, 20, 1, 25));
            AssertMatchToken(TokenType.NVSingleQuote, new Position(1, 25, 1, 26));
            AssertMatchToken(TokenType.NVRParen, new Position(1, 26, 1, 27));
        }
    }
}