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
    public class XmlTestCase : ScannerTestBase
    {
        [Test]
        public void EmptySource()
        {
            _scanner.SetSource(
                "");

            AssertEOF();
        }

        [Test]
        public void SingleWellFormedTag()
        {
            _scanner.SetSource(
                "<name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void SingleWellFormedSelfClosingTag()
        {
            _scanner.SetSource(
                "<name/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedStartAndEndTags()
        {
            _scanner.SetSource(
                "<name></name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedStartAndEndTagsWithSimpleContent()
        {
            _scanner.SetSource(
                "<name>text with spaces</name>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text with spaces");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedTagContainingNewLine()
        {
            _scanner.SetSource(
                "<name\n" +
                ">");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "name");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void XmlDeclaration()
        {
            _scanner.SetSource(
                "<?xml version=\"1.0\" ?>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagName, "xml");
            AssertMatchToken(TokenType.XmlAttributeName, "version");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "1.0");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void XmlDeclarationInLineScanner()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "<?xml version=\"1.0\" ?>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagName, "xml");
            AssertMatchToken(TokenType.XmlAttributeMemberSelect, " ");
            AssertMatchToken(TokenType.XmlAttributeName, "version");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "1");
            AssertMatchToken(TokenType.XmlAttributeText, ".");
            AssertMatchToken(TokenType.XmlAttributeText, "0");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeMemberSelect, " ");
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void DocTypeDeclaration()
        {
            _scanner.SetSource(
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" " +
                "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlExclaimationMark);
            AssertMatchToken(TokenType.XmlTagName, "DOCTYPE");
            AssertMatchToken(TokenType.XmlAttributeName, "html");
            AssertMatchToken(TokenType.XmlAttributeName, "PUBLIC");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "-//W3C//DTD XHTML 1.0 Transitional//EN");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ProcessingInstruction()
        {
            _scanner.SetSource(
                "<?xml-stylesheet href=\"style.css\" type=\"text/css\"?>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagName, "xml-stylesheet");
            AssertMatchToken(TokenType.XmlAttributeName, "href");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "style.css");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeName, "type");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "text/css");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlQuestionMark);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void WellFormedOpenAndCloseComment()
        {
            _scanner.SetSource(
                "before<!-- inside -->after");

            AssertMatchToken(TokenType.XmlText, "before");
            AssertMatchToken(TokenType.XmlComment, "<!-- inside -->");
            AssertMatchToken(TokenType.XmlText, "after");

            AssertEOF();
        }

        [Test]
        public void FirstThreeCharactersOfCommentIsAnElement()
        {
            _scanner.SetSource(
                "<!- notAComment");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlExclaimationMark);

            _scanner.GetToken();

            // Add an error because a '-' is not allowed in a tag
            Assert.AreEqual(1, _scanner.Errors.Count);
        }

        [Test]
        public void TagTokenCharsScannedAsXmlTextInDefaultState()
        {
            _scanner.SetSource(
                "> / \" ? !");

            AssertMatchToken(TokenType.XmlText, "> / \" ? !");

            AssertEOF();
        }

        [Test]
        public void CDataSection()
        {
            _scanner.SetSource(
                "<![CDATA[ text ]]>");

            AssertMatchToken(TokenType.XmlCDataStart, "<![CDATA[");
            AssertMatchToken(TokenType.XmlCDataSection, " text ");
            AssertMatchToken(TokenType.XmlCDataEnd, "]]>");

			AssertEOF();
        }

        [Test]
        public void CDataSectionContainingTag()
        {
            _scanner.SetSource(
                "<![CDATA[ <tag> ]]>");

            AssertMatchToken(TokenType.XmlCDataStart, "<![CDATA[");
            AssertMatchToken(TokenType.XmlCDataSection, " <tag> ");
            AssertMatchToken(TokenType.XmlCDataEnd, "]]>");

			AssertEOF();
        }

        [Test]
        public void CDataSectionWithMissingEnd()
        {
            _scanner.SetSource(
                "<![CDATA[ text inside the section ");

            AssertMatchToken(TokenType.XmlCDataStart, "<![CDATA[");
            
            // Throws a ScannerError because no end can be found when scanning for the content token
            try
            {
                _scanner.GetToken();
                Assert.Fail(); // Should not get to this point
            }
            catch (ScannerError) { }
        }

        [Test]
        public void CDataSectionWithinScriptElementIsScanned()
        {
            _scanner.SetSource(
                "<script><![CDATA[ JS Here ]]></script>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlCDataStart);
            AssertMatchToken(TokenType.XmlCDataSection, " JS Here ");
            AssertMatchToken(TokenType.XmlCDataEnd);

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "script");
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void HashNotFollowedByTextIsXmlText()
        {
            _scanner.SetSource(
                "<td>#</td>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "#");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void HashPrecededByTextNotFollowedByTextIsXmlText()
        {
            _scanner.SetSource(
                "<td>text#</td>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertMatchToken(TokenType.XmlText, "text#");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagName, "td");
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void HashAtBeginningOfXmlTagAttributeValue()
        {
            _scanner.SetSource(
                "<a title=\"#\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "#");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void HashWithinXmlTagAttributeValue()
        {
            _scanner.SetSource(
                "<a title=\"You are #1.\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "You are #1.");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void DollarSignThatIsNotNVReference()
        {
            _scanner.SetSource(
                "$");

            AssertMatchToken(TokenType.XmlText, "$");

			AssertEOF();
        }

        [Test]
        public void DollarSignWithinXmlTextThatIsNotNVReference()
        {
            _scanner.SetSource(
                "I have $100.");

            AssertMatchToken(TokenType.XmlText, "I have $100.");

			AssertEOF();
        }

        [Test]
        public void DollarSignWithinXmlTagAttributeValueThatIsNotNVReference()
        {
            _scanner.SetSource(
                "<a title=\"I have $100.\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeName, "title");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "I have $100.");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

        [Test]
        public void XmlAttributeWithSingleQuotes()
        {
            _scanner.SetSource(
                "<tag attr='before\"after' />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "tag");
            AssertMatchToken(TokenType.XmlAttributeName);
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlSingleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "before\"after");
            AssertMatchToken(TokenType.XmlSingleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

			AssertEOF();
        }

		[Test]
		public void IncompleteXmlTagWithNoNameInsideXmlTagBreaksOutOfXmlTagWhenInIntelliSenseMode()
		{
			Scanner scanner = new Scanner(new ErrorHandler());
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			scanner.Options = scannerOptions;

			scanner.SetSource(
				"<div>\n" +
				"    < \n" +
				"</div>");

			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlTagName, "div");
			AssertMatchToken(scanner, TokenType.XmlTagEnd);

			AssertMatchToken(scanner, TokenType.XmlText, "\n    ");
			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlAttributeMemberSelect, " ");
			AssertMatchToken(scanner, TokenType.XmlAttributeMemberSelect, "\n");
			// When in IntelliSense mode the scanner should drop out of a tag if it finds a '<'

			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlForwardSlash);
			AssertMatchToken(scanner, TokenType.XmlTagName, "div");
			AssertMatchToken(scanner, TokenType.XmlTagEnd);

			AssertEOF();
		}

		[Test]
		public void IncompleteXmlTagInsideXmlTagBreaksOutOfXmlTagWhenInIntelliSenseMode()
		{
			Scanner scanner = new Scanner(new ErrorHandler());
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			scanner.Options = scannerOptions;

			scanner.SetSource(
				"<div>\n" +
				"    <a \n" +
				"</div>");

			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlTagName, "div");
			AssertMatchToken(scanner, TokenType.XmlTagEnd);

			AssertMatchToken(scanner, TokenType.XmlText, "\n    ");
			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlTagName, "a");
			AssertMatchToken(scanner, TokenType.XmlAttributeMemberSelect, " ");
			AssertMatchToken(scanner, TokenType.XmlAttributeMemberSelect, "\n");
			// When in IntelliSense mode the scanner should drop out of a tag if it finds a '<'

			AssertMatchToken(scanner, TokenType.XmlTagStart);
			AssertMatchToken(scanner, TokenType.XmlForwardSlash);
			AssertMatchToken(scanner, TokenType.XmlTagName, "div");
			AssertMatchToken(scanner, TokenType.XmlTagEnd);

			AssertEOF();
		}

        [Test]
        public void HashAtBeginningOfXmlTagAttributeValueFollowedByMoreAttributeContentIsDirectiveInTelliSenseMode()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.Options.IsLineScanner = true;
            _scanner.SetSource(
                "<a href=\"# \"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeMemberSelect, " ");
            AssertMatchToken(TokenType.XmlAttributeName, "href");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.NVDirectiveHash, "#");
            AssertMatchToken(TokenType.XmlAttributeText, " ");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void HashAtBeginningOfXmlTagAttributeValueDirectlyFollowedByQuoteIsXmlTextInIntelliSenseMode()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.SplitTextTokens = true;
            _scanner.SetSource(
                "<a href=\"#\"/>");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeMemberSelect, " ");
            AssertMatchToken(TokenType.XmlAttributeName, "href");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlAttributeText, "#");
            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }
    }
}