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
    public class NVelocityTokenTestCase : ScannerTestBase
    {
        [Test]
        public void DollarAndIdentifierInDirectiveParams()
        {
            _scanner.SetSource(
                "#if($varName)");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName");
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void TwoReferencesInDirectiveParams()
        {
            _scanner.SetSource(
                "#if($varName1 $varName2)");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName1");
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "varName2");
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void OpeningAndClosingBrackets()
        {
            _scanner.SetSource(
                "#if( [ ] )");

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);

            AssertMatchToken(TokenType.NVLBrack);
            AssertMatchToken(TokenType.NVRBrack);

            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertEOF();
        }

        [Test]
        public void ReferenceInsideXmlTag()
        {
            _scanner.SetSource(
                "<div $var.Call() />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Call");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);

            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ReferenceInsideXmlAttributeValue()
        {
            _scanner.SetSource(
                "<div class=\"$var.Call()\" />");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "div");
            AssertMatchToken(TokenType.XmlAttributeName, "class");
            AssertMatchToken(TokenType.XmlEquals);
            AssertMatchToken(TokenType.XmlDoubleQuote);

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Call");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);

            AssertMatchToken(TokenType.XmlDoubleQuote);
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            AssertEOF();
        }

        [Test]
        public void ReferenceWithSelectorFollowedByText()
        {
            _scanner.SetSource(
                "$var.Method()text");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVRParen);
            
            AssertMatchToken(TokenType.XmlText, "text");

            AssertEOF();
        }

        [Test]
        public void NVStringLiteralWithDollarAmountShouldBeASingleToken()
        {
            _scanner.SetSource(
                "$a.B(\"$100\")");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "B");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVStringLiteral, "$100");
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVRParen);

            AssertEOF();
        }

        [Test]
        public void NVStringLiteralWithDirectiveShouldBeScanned()
        {
            _scanner.SetSource(
                "$a.B(\"#if(true)$a#end\")");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "B");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVTrue);
            AssertMatchToken(TokenType.NVDirectiveRParen);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");

            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVRParen);

            AssertEOF();
        }

        [Test]
        public void TwoDotsFollowingAnIdentifierAreScannedAsADoubleDot()
        {
            _scanner.SetSource(
                "$a.B([$i..$n])");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "a");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "B");
            AssertMatchToken(TokenType.NVLParen);

            AssertMatchToken(TokenType.NVLBrack);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "i");
            AssertMatchToken(TokenType.NVDoubleDot);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "n");
            AssertMatchToken(TokenType.NVRBrack);

            AssertMatchToken(TokenType.NVRParen);

            AssertEOF();
        }

        [Test]
        public void NVDictionaryDoesNotConsumeRCurlyOnIdentifierWithoutInitialLCurly()
        {
            _scanner.SetSource(
                "$obj.Method(\"%{key=$value}\")");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "obj");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");
            AssertMatchToken(TokenType.NVLParen);

            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVDictionaryPercent);
            AssertMatchToken(TokenType.NVDictionaryLCurly);
            AssertMatchToken(TokenType.NVDictionaryKey, "key");
            AssertMatchToken(TokenType.NVDictionaryEquals);
            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "value");
            AssertMatchToken(TokenType.NVDictionaryRCurly);
            AssertMatchToken(TokenType.NVDoubleQuote);

            AssertMatchToken(TokenType.NVRParen);

            AssertEOF();
        }

        [Test]
        public void ReferenceFollowedByTextOnFollowingLine()
        {
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.SplitTextTokens = true;
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;

            _scanner.SetSource(
                "$var.Method");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");

            _scanner.RestoreState(_scanner.RetrieveState());

            _scanner.SetSource(
                "text");

            AssertMatchToken(TokenType.XmlText, "text");

            AssertEOF();
        }

        [Test]
        public void IncompleteMethodCallIsClosedInIntelliSenseMode()
        {
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;

            _scanner.SetSource(
                "$var.Method(\n" +
                "text");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.XmlText, "\ntext");

            Assert.AreEqual(ScannerState.Default, _scanner.RetrieveState().Peek());
        }

        [Test]
        public void IncompleteDictionaryIsClosedInIntelliSenseMode()
        {
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;

            _scanner.SetSource(
                "$Form.TextField(\"customer.LastName\", \"%\n" +
                "<a />");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "Form");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "TextField");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVStringLiteral, "customer.LastName");
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVComma);
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVDictionaryPercent);

            AssertMatchToken(TokenType.XmlText, "\n");
            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "a");
            AssertMatchToken(TokenType.XmlAttributeMemberSelect, " ");
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);

            Assert.AreEqual(ScannerState.Default, _scanner.RetrieveState().Peek());
        }

        [Test]
        public void IncompleteDictionaryWithBraceIsClosedInIntelliSenseMode()
        {
            _scanner.Options.IsLineScanner = true;
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;

            _scanner.SetSource(
                "$var.Method(\"%{\n" +
                "text");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "var");
            AssertMatchToken(TokenType.NVDot);
            AssertMatchToken(TokenType.NVIdentifier, "Method");
            AssertMatchToken(TokenType.NVLParen);
            AssertMatchToken(TokenType.NVDoubleQuote);
            AssertMatchToken(TokenType.NVDictionaryPercent);
            AssertMatchToken(TokenType.NVDictionaryLCurly);
            AssertMatchToken(TokenType.XmlText, "\ntext");

            Assert.AreEqual(ScannerState.Default, _scanner.RetrieveState().Peek());
        }
    }
}