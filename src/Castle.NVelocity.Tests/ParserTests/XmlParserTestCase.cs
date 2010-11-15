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

namespace Castle.NVelocity.Tests.ParserTests
{
    using Castle.NVelocity.Ast;
    using NUnit.Framework;

    [TestFixture]
    public class XmlParserTestCase : ParserTestBase
    {
        [Test]
        public void ParseEmptyDocument()
        {
            Parser parser = GetNewParser("");
            TemplateNode template = parser.ParseTemplate();

            // Template has no children
            Assert.AreEqual(0, template.Content.Count);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlText()
        {
            Parser parser = GetNewParser("This is some XmlText");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 XML text node
            Assert.AreEqual(1, template.Content.Count);
            Assert.AreEqual(typeof(XmlTextNode), template.Content[0].GetType());
            XmlTextNode xmlTextNode = (XmlTextNode) template.Content[0];
            Assert.AreEqual("This is some XmlText", xmlTextNode.Text);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlTagOpenAsElement()
        {
            Parser parser = GetNewParser("<");
            TemplateNode templateNode = parser.ParseTemplate();

            // Template has 1 element
            Assert.AreEqual(1, templateNode.Content.Count);
            Assert.AreEqual("", ((XmlElement)templateNode.Content[0]).Name);
            AssertPosition(new Position(1, 1, 1, 2), templateNode.Content[0].Position);
        }

        [Test]
        public void ParseSelfClosingElement()
        {
            Parser parser = GetNewParser("<tag/>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 self closing element
            Assert.AreEqual(1, template.Content.Count);
            Assert.AreEqual(typeof(XmlElement), template.Content[0].GetType());
            XmlElement xmlElement = (XmlElement) template.Content[0];
            Assert.AreEqual("tag", xmlElement.Name);
            Assert.AreEqual(true, xmlElement.IsSelfClosing);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseElementWithNoContent()
        {
            Parser parser = GetNewParser("<tag></tag>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 empty content element
            Assert.AreEqual(1, template.Content.Count);
            Assert.AreEqual(typeof(XmlElement), template.Content[0].GetType());
            XmlElement xmlElement = (XmlElement) template.Content[0];
            Assert.AreEqual("tag", xmlElement.Name);
            Assert.AreEqual(false, xmlElement.IsSelfClosing);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseElementWithTextContent()
        {
            Parser parser = GetNewParser("<tag>text content</tag>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 element
            Assert.AreEqual(1, template.Content.Count);
            Assert.AreEqual(typeof(XmlElement), template.Content[0].GetType());
            XmlElement xmlElement = (XmlElement)template.Content[0];
            Assert.AreEqual("tag", xmlElement.Name);
            Assert.AreEqual(false, xmlElement.IsSelfClosing);

            // With text content
            Assert.AreEqual(1, xmlElement.Content.Count);
            Assert.AreEqual(typeof(XmlTextNode), xmlElement.Content[0].GetType());
            XmlTextNode xmlTextNode = (XmlTextNode) xmlElement.Content[0];
            Assert.AreEqual("text content", xmlTextNode.Text);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNestedElements()
        {
            Parser parser = GetNewParser("<tag><nested></nested></tag>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 element
            Assert.AreEqual(1, template.Content.Count);
            XmlElement xmlElement = (XmlElement) template.Content[0];

            // With another element nested within
            Assert.AreEqual(1, xmlElement.Content.Count);
            XmlElement nestedXmlElement = (XmlElement) xmlElement.Content[0];
            Assert.AreEqual(0, nestedXmlElement.Content.Count);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNestedElementsWithNVReferenceBetweenClosingTags()
        {
            Parser parser = GetNewParser("<root><nested></nested>$reference</root>");
            TemplateNode templateNode = parser.ParseTemplate();

            // 'tag' XML Element has 2 content nodes
            XmlElement rootElement = (XmlElement)templateNode.Content[0];
            Assert.AreEqual(2, rootElement.Content.Count);
            Assert.AreEqual("nested", ((XmlElement)rootElement.Content[0]).Name);
            Assert.AreEqual("reference", ((NVReference)rootElement.Content[1]).Designator.Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlElementWithEmptyAttributeValue()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "<tag attr=\"\" />");
            TemplateNode templateNode = parser.ParseTemplate();

            // Element has 1 attribute with no content nodes
            XmlAttribute xmlAttribute = ((XmlAttribute)((XmlElement)templateNode.Content[0]).Attributes[0]);
            Assert.AreEqual(0, xmlAttribute.Content.Count);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlElementWithAttributes()
        {
            Parser parser = GetNewParser("<tag attr=\"value\"/>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 XML Element
            XmlElement xmlElement = (XmlElement) template.Content[0];

            // Element has 1 Attribute
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            XmlAttribute xmlAttribute = (XmlAttribute) xmlElement.Attributes[0];
            Assert.AreEqual("attr", xmlAttribute.Name);

            // Element has 1 XML text node
            Assert.AreEqual(1, xmlAttribute.Content.Count);
            XmlTextNode xmlTextNode = (XmlTextNode) xmlAttribute.Content[0];
            Assert.AreEqual("value",  xmlTextNode.Text);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlElementWithTwoAttributes()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "<tag attr1=\"\" attr2=\"\" />");
            TemplateNode templateNode = parser.ParseTemplate();

            // XML Element has 2 attributes
            XmlElement xmlElement = (XmlElement)templateNode.Content[0];
            Assert.AreEqual(2, xmlElement.Attributes.Count);
            Assert.AreEqual("attr1", ((XmlAttribute)xmlElement.Attributes[0]).Name);
            Assert.AreEqual("attr2", ((XmlAttribute)xmlElement.Attributes[1]).Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseXmlElementWithSingleQuotesAroundAttribute()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "<tag attr='value'/>");
            TemplateNode template = parser.ParseTemplate();

            // Template has 1 XML Element
            XmlElement xmlElement = (XmlElement)template.Content[0];

            // Element has 1 Attribute
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            XmlAttribute xmlAttribute = (XmlAttribute)xmlElement.Attributes[0];
            Assert.AreEqual("attr", xmlAttribute.Name);

            // Element has 1 XML text node
            Assert.AreEqual(1, xmlAttribute.Content.Count);
            XmlTextNode xmlTextNode = (XmlTextNode)xmlAttribute.Content[0];
            Assert.AreEqual("value",  xmlTextNode.Text);

            AssertNoErrors(parser);
        }

        [Test]
        public void XmlElementsHaveParentElements()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "<outer><inner></inner></outer>");
            TemplateNode templateNode = parser.ParseTemplate();

            Assert.AreEqual(
                templateNode.Content[0],
                ((XmlElement)((XmlElement)templateNode.Content[0]).Content[0]).Parent);

            AssertNoErrors(parser);
        }

        [Test]
        public void XmlElementIsCompleteWhenSyntaxicallyCorrect()
        {
            Parser parser = GetNewParser("<tag></tag>");
            TemplateNode templateNode = parser.ParseTemplate();

            Assert.IsTrue(((XmlElement)templateNode.Content[0]).IsComplete);

            AssertNoErrors(parser);
        }

        [Test]
        public void XmlElementIsNotCompleteWhenSyntaxicallyIncomplete()
        {
            Parser parser = GetNewParser("<tag><");
            TemplateNode templateNode = parser.ParseTemplate();

            Assert.IsFalse(((XmlElement)templateNode.Content[0]).IsComplete);
        }

		[Test]
		public void XmlAttributeIsNotCompleteWhenSyntaxicallyIncomplete()
		{
			Parser parser = GetNewParser("<tag attr=");
			TemplateNode templateNode = parser.ParseTemplate();

			Assert.IsFalse(((XmlElement)templateNode.Content[0]).IsComplete);
		}
    }
}