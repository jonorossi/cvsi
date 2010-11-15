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

namespace Castle.NVelocity.Tests.ParserTests
{
	using NUnit.Framework;
	using Castle.NVelocity.Ast;

	[TestFixture]
	public class XmlPositionTestCase : ParserTestBase
	{
		[Test]
		public void XmlTextNodePosition()
		{
			Parser parser = GetNewParser("this is some text");
			TemplateNode templateNode = parser.ParseTemplate();

			// Check XmlTextNode position
			XmlTextNode xmlTextNode = (XmlTextNode)templateNode.Content[0];
			AssertPosition(new Position(1, 1, 1, 18), xmlTextNode.Position);
		}

        [Test]
        public void XmlElementSelfClosingPosition()
        {
            Parser parser = GetNewParser("<element />");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check XmlElement position
            XmlElement xmlElement = (XmlElement) templateNode.Content[0];
            AssertPosition(new Position(1, 1, 1, 12), xmlElement.Position);
        }

        [Test]
        public void XmlElementWithXmlTextNodeInContentPosition()
        {
            Parser parser = GetNewParser("<element>some text</element>");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check XmlElement position
            XmlElement xmlElement = (XmlElement) templateNode.Content[0];
            AssertPosition(new Position(1, 1, 1, 29), xmlElement.Position);

            // Check XmlTextNode position
            XmlTextNode xmlTextNode = (XmlTextNode) xmlElement.Content[0];
            AssertPosition(new Position(1, 10, 1, 19), xmlTextNode.Position);
        }

        [Test]
        public void XmlElementWithSurroundingText()
        {
            Parser parser = GetNewParser(
                "before<p>inside</p>after");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check preceding XmlTextNodePosition
            AssertPosition(new Position(1, 1, 1, 7), templateNode.Content[0].Position);

            // Check XmlElement
            AssertPosition(new Position(1, 7, 1, 20), templateNode.Content[1].Position);
            //TODO: AssertPosition(new Position(1, 7, 1, 10), ((XmlElement)templateNode.Content[1]).StartTagPosition);
            //TODO: AssertPosition(new Position(1, 16, 1, 20), ((XmlElement)templateNode.Content[1]).EndTagPosition);

            // Check following XmlTextNodePosition
            AssertPosition(new Position(1, 20, 1, 25), templateNode.Content[2].Position);
        }
    }
}