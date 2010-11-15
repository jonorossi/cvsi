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
    using Castle.NVelocity.Ast;
    using NUnit.Framework;

    [TestFixture]
    public class GetNodeAtPositionTestCase : ParserTestBase
    {
        [Test]
        public void StartOfDesignator()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "$");
            //    ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Check template
            Assert.AreEqual(1, templateNode.Content.Count);
            Assert.AreEqual(typeof(NVReference), templateNode.Content[0].GetType());

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 2);
            Assert.AreEqual(typeof(NVDesignator), astNode.GetType());
            AssertPosition(new Position(1, 1, 1, 2), astNode.Position);
        }

        [Test]
        public void FirstDesignatorSelector()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "$Ajax.");
            //         ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Check template
            NVReference nvReference = (NVReference)templateNode.Content[0];
            Assert.AreEqual("Ajax", nvReference.Designator.Name);
            NVSelector nvSelector = nvReference.Designator.Selectors[0];
            Assert.AreEqual("", nvSelector.Name);

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 7);
            Assert.AreEqual(typeof(NVSelector), astNode.GetType());
            AssertPosition(new Position(1, 6, 1, 7), astNode.Position);
        }

        [Test]
        public void SecondDesignatorSelector()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "$Ajax.InstallScripts.");
            //                        ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 22);
            Assert.AreEqual(typeof(NVSelector), astNode.GetType());
            AssertPosition(new Position(1, 21, 1, 22), astNode.Position); // selectors include '.'
        }

        [Test]
        public void ReferenceFollowedByText()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "$Ajax.InstallScripts() text\n        text");
            //                          ^      ^
            TemplateNode templateNode = parser.ParseTemplate();

            // XmlTextNode, it is only one text node
            XmlTextNode point1 = (XmlTextNode)templateNode.GetNodeAt(1, 24);
            XmlTextNode point2 = (XmlTextNode)templateNode.GetNodeAt(2, 2);
            AssertPosition(new Position(1, 23, 2, 13), point1.Position);
            AssertPosition(new Position(1, 23, 2, 13), point2.Position);
        }

        [Test]
        public void ReferenceFollowedByReference()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "$Ajax.InstallScripts()\n$Form.");
            //                                 ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Designator
            NVSelector designator = (NVSelector)templateNode.GetNodeAt(2, 7);
            AssertPosition(new Position(2, 6, 2, 7), designator.Position);
        }

        [Test]
        public void StartOfDirective()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "#");
            //    ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Check template
            NVDirective nvDirective = (NVDirective)templateNode.Content[0];
            Assert.AreEqual("", nvDirective.Name);

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 2);
            Assert.AreEqual(typeof(NVDirective), astNode.GetType());
            AssertPosition(new Position(1, 1, 1, 2), astNode.Position);
        }

        [Test]
        public void InsideComponent()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "#component(");
            //              ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Check template
            NVDirective nvDirective = (NVDirective)templateNode.Content[0];
            Assert.AreEqual("component", nvDirective.Name);

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 12);
            Assert.AreEqual(typeof(NVDirective), astNode.GetType());
            AssertPosition(new Position(1, 1, 1, 12), astNode.Position);
        }

        [Test]
        public void InsideBodyOfComponentAtEndOfStartedDirective()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "#foreach($item in $items)\n" +
                "    #\n" +
                //    ^
                "#end");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check the inner directive
            NVDirective nvDirective = (NVDirective)((NVForeachDirective)templateNode.Content[0]).Content[1];
            Assert.AreEqual("", nvDirective.Name);

            // Get the node at the location and check its position
            AstNode astNode = templateNode.GetNodeAt(2, 6);
            Assert.IsInstanceOfType(typeof(NVDirective), astNode);
            AssertPosition(new Position(2, 5, 3, 1), astNode.Position);
        }

        [Test]
        public void InsideStringLiteralInReference()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
                "$obj.Method(\"");
            //                 ^
            TemplateNode templateNode = parser.ParseTemplate();

            // Get node at position
            AstNode astNode = templateNode.GetNodeAt(1, 14);

            //TODO finish
            //NVStringExpression stringExpr = (NVStringExpression)astNode;
            //Assert.AreEqual("", stringExpr.Value);
            //AssertPosition(new Position(1, 14, 1, 14), stringExpr.Position);
        }

        [Test]
        public void InsideWhitespacePrecededByXmlElement()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
                "  <p>inside</p>  ");
            //    ^              ^
            TemplateNode templateNode = parser.ParseTemplate();

            AstNode astNode;

            // Get the first XmlTextNode
            astNode = templateNode.GetNodeAt(1, 2);
            Assert.AreEqual("  ", ((XmlTextNode)astNode).Text);

            // Get the second XmlTextNode
            astNode = templateNode.GetNodeAt(1, 17);
            Assert.AreEqual("  ", ((XmlTextNode)astNode).Text);
        }

		[Test]
		public void AtEndOfUnclosedXmlElementWithNoAttributes()
		{
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
				"<a      ");
			//      ^
			TemplateNode templateNode = parser.ParseTemplate();

			AstNode astNode = templateNode.GetNodeAt(1, 4);
			Assert.IsAssignableFrom(typeof(XmlElement), astNode);
			AssertPosition(new Position(1, 1, 1, 9), astNode.Position);
		}

		[Test]
		public void AtEndOfUnclosedXmlElementWithAtLeastOneAttribute()
		{
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
				"<a name=\"\"     ");
			//               ^
			TemplateNode templateNode = parser.ParseTemplate();

			AstNode astNode = templateNode.GetNodeAt(1, 11);
			Assert.IsAssignableFrom(typeof(XmlElement), astNode);
			AssertPosition(new Position(1, 1, 1, 16), astNode.Position);
		}

		[Test]
		public void AtEndOfUnclosedXmlTagInsideXmlContent()
		{
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
				"<div>\n" +
				"    <a \n" +
				//      ^
				"</div>");
			TemplateNode templateNode = parser.ParseTemplate();

			AstNode astNode = templateNode.GetNodeAt(2, 7);
			Assert.IsAssignableFrom(typeof(XmlElement), astNode);
			AssertPosition(new Position(2, 5, 2, 8), astNode.Position);
		}

		[Test]
		public void AtEndOfUnclosedXmlTagWithOneAttributeInsideXmlContent()
		{
			ScannerOptions scannerOptions = new ScannerOptions();
			scannerOptions.EnableIntelliSenseTriggerTokens = true;
			Parser parser = GetNewParser(scannerOptions,
				"<div>\n" +
				"    <a name=\"\" \n" +
				//                ^
				"</div>");
			TemplateNode templateNode = parser.ParseTemplate();

			AstNode astNode = templateNode.GetNodeAt(2, 16);
			Assert.IsAssignableFrom(typeof(XmlElement), astNode);
			AssertPosition(new Position(2, 5, 2, 16), astNode.Position);
		}
	}
}