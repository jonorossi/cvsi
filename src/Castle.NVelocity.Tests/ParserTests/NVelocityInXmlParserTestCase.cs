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
    using Ast;
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityInXmlParserTestCase : ParserTestBase
    {
        [Test]
        public void ParseNVelocityInXmlTagAttributes()
        {
            Parser parser = GetNewParser("<tag $var />");
            TemplateNode templateNode = parser.ParseTemplate();

            // Template has 1 XML Element
            Assert.AreEqual(1, templateNode.Content.Count);
            XmlElement xmlElement = (XmlElement) templateNode.Content[0];
            Assert.AreEqual("tag", xmlElement.Name);

            // With 1 NVReference as an attribute
            Assert.AreEqual(1, xmlElement.Attributes.Count);
            Assert.AreEqual(typeof(NVReference), xmlElement.Attributes[0].GetType());
            NVReference nvReference = (NVReference) xmlElement.Attributes[0];
            Assert.AreEqual("var", nvReference.Designator.Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNVelocityInXmlElementBody()
        {
            Parser parser = GetNewParser("<p>$var.Method</p>");
            TemplateNode templateNode = parser.ParseTemplate();

            // Template has 1 XML Element
            Assert.AreEqual(typeof(XmlElement), templateNode.Content[0].GetType());

            // XML Element has a NVReference
            Assert.AreEqual(typeof(NVReference), ((XmlElement) templateNode.Content[0]).Content[0].GetType());

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNVelocityReferenceInXmlAttribute()
        {
            Parser parser = GetNewParser("<img src=\"$siteRoot\"/>");
            TemplateNode templateNode = parser.ParseTemplate();

            // Template has 1 XML Element
            Assert.AreEqual("img", ((XmlElement)templateNode.Content[0]).Name);

            // XML Element has a NVReference
            XmlAttribute imgAttr = (XmlAttribute)((XmlElement)templateNode.Content[0]).Attributes[0];
            Assert.AreEqual("siteRoot", ((NVReference)imgAttr.Content[0]).Designator.Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNVelocityReferenceAndXmlTextInXmlAttribute()
        {
            Parser parser = GetNewParser("<img src=\"${siteRoot}/image.png\"/>");
            TemplateNode templateNode = parser.ParseTemplate();

            // Template has 1 XML Element
            XmlAttribute xmlAttribute = ((XmlAttribute)((XmlElement)templateNode.Content[0]).Attributes[0]);

            // XML Element has 1 attribute with a NVReference and a XmlText node
            Assert.AreEqual(2, xmlAttribute.Content.Count);
            Assert.AreEqual("siteRoot", ((NVReference)xmlAttribute.Content[0]).Designator.Name);
            Assert.AreEqual("/image.png", ((XmlTextNode)xmlAttribute.Content[1]).Text);

            AssertNoErrors(parser);
        }
    }
}