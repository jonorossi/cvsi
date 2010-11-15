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
    public class TokenLookAhead : ScannerTestBase
    {
        [Test]
        public void CanPeekAheadOneTokenThenCallGetToken()
        {
            _scanner.SetSource(
                "text<tag/>");

            AssertMatchToken(TokenType.XmlText, "text");
            AssertMatchToken(TokenType.XmlTagStart);
            
            AssertMatchToken(TokenType.XmlTagName, _scanner.PeekToken(1));
            AssertMatchToken(TokenType.XmlTagName, "tag");

            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void CanPeekAheadFourTokensThenCallGetToken()
        {
            _scanner.SetSource(
                "<tag/>");

            AssertMatchToken(TokenType.XmlTagStart, _scanner.PeekToken(1));
            AssertMatchToken(TokenType.XmlTagName, _scanner.PeekToken(2));
            AssertMatchToken(TokenType.XmlForwardSlash, _scanner.PeekToken(3));
            AssertMatchToken(TokenType.XmlTagEnd, _scanner.PeekToken(4));

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "tag");
            AssertMatchToken(TokenType.XmlForwardSlash);
            AssertMatchToken(TokenType.XmlTagEnd);
        }

        [Test]
        public void CanPeekAheadToDetermineIfTheDirectiveIsAnElseIfOrAnEnd()
        {
            _scanner.SetSource(
                "#if ()   #elseif ()   #end");

            // #if
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "if");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlText, "   ");

            // #elseif
            AssertMatchToken(TokenType.NVDirectiveHash, _scanner.PeekToken(1));
            AssertMatchToken(TokenType.NVDirectiveName, "elseif", _scanner.PeekToken(2));

            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "elseif");
            AssertMatchToken(TokenType.NVDirectiveLParen);
            AssertMatchToken(TokenType.NVDirectiveRParen);

            AssertMatchToken(TokenType.XmlText, "   ");

            // #end
            AssertMatchToken(TokenType.NVDirectiveHash);
            AssertMatchToken(TokenType.NVDirectiveName, "end");
        }

        [Test]
        public void CanPeekAheadOneThenPeekAheadOneAgain()
        {
            _scanner.SetSource(
                "<tag/>");

            AssertMatchToken(TokenType.XmlTagStart, _scanner.PeekToken(1));
            AssertMatchToken(TokenType.XmlTagStart, _scanner.PeekToken(1));
            AssertMatchToken(TokenType.XmlTagName, "tag", _scanner.PeekToken(2));
            AssertMatchToken(TokenType.XmlForwardSlash, _scanner.PeekToken(3));
            AssertMatchToken(TokenType.XmlTagEnd, _scanner.PeekToken(4));

            AssertMatchToken(TokenType.XmlTagStart);
        }

        // prop -> eof
    }
}
