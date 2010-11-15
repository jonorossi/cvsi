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
    public class ScannerOptionsTestCase : ScannerTestBase
    {
        [Test]
        public void EnableIntelliSenseTriggerTokens_ScansDollarAsNVDollar()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.SetSource(
                "$");

            AssertMatchToken(TokenType.NVDollar);

            AssertEOF();
        }

        [Test]
        public void EnableIntelliSenseTriggerTokens_ScansDotAsNVDot()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.SetSource(
                "$Ajax.");

            AssertMatchToken(TokenType.NVDollar);
            AssertMatchToken(TokenType.NVIdentifier, "Ajax");
            AssertMatchToken(TokenType.NVDot);

            AssertEOF();
        }

        [Test]
        public void EnableIntelliSenseTriggerTokens_ScansXmlAttributeMemberSelectSpaces()
        {
            _scanner.Options.EnableIntelliSenseTriggerTokens = true;
            _scanner.SetSource(
                "<tag ");

            AssertMatchToken(TokenType.XmlTagStart);
            AssertMatchToken(TokenType.XmlTagName, "tag");
            AssertMatchToken(TokenType.XmlAttributeMemberSelect);
        }
    }
}