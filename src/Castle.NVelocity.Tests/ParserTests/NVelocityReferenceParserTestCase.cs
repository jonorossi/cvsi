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
    public class NVelocityReferenceParserTestCase : ParserTestBase
    {
        [Test]
        public void ParseReferenceWithOneIdentfierSelector()
        {
            Parser parser = GetNewParser("$first.second");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference has 1 selector
            Assert.AreEqual(1, templateNode.Content.Count);
            NVReference nvReference = (NVReference) templateNode.Content[0];
            Assert.AreEqual("first", nvReference.Designator.Name);
            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);
            Assert.AreEqual("second", nvReference.Designator.Selectors[0].Name);
        }

        [Test]
        public void ParseReferenceWithTwoSelectors()
        {
            Parser parser = GetNewParser("$first.second.third");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference has 2 selectors
            Assert.AreEqual(1, templateNode.Content.Count);
            NVReference nvReference = (NVReference)templateNode.Content[0];
            AssertPosition(new Position(1, 1, 1, 20), nvReference.Position);

            Assert.AreEqual(2, nvReference.Designator.Selectors.Count);
            Assert.AreEqual("third", nvReference.Designator.Selectors[1].Name);
            AssertPosition(new Position(1, 7, 1, 14), nvReference.Designator.Selectors[0].Position);
            AssertPosition(new Position(1, 14, 1, 20), nvReference.Designator.Selectors[1].Position);
        }

        [Test]
        public void ParseReferenceWithEmptyActualParams()
        {
            Parser parser = GetNewParser("$first.second()");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference with 1 selector and no actual params
            Assert.AreEqual(1, templateNode.Content.Count);
            NVReference nvReference = (NVReference) templateNode.Content[0];
            Assert.AreEqual("first", nvReference.Designator.Name);
            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);
            Assert.AreEqual(0, nvReference.Designator.Selectors[0].Actuals.Count);
        }

        [Test]
        public void ParseReferenceWithOneActualParam()
        {
            Parser parser = GetNewParser("$first.second(100)");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference with 1 selector and 1 actual param
            NVReference nvReference = (NVReference) templateNode.Content[0];
            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);
            NVSelector nvSelector = nvReference.Designator.Selectors[0];
            Assert.AreEqual(1, nvSelector.Actuals.Count);
            Assert.AreEqual(typeof(NVNumExpression), nvSelector.Actuals[0].GetType());
            NVNumExpression numExpr = (NVNumExpression) nvSelector.Actuals[0];
            Assert.AreEqual(100, numExpr.Value);
        }

        [Test]
        public void ParseReferenceWithMultipleActualParams()
        {
            Parser parser = GetNewParser("$first.second(100, 200, 300)");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference with 1 selector and 1 actual param
            NVSelector nvSelector = ((NVReference)templateNode.Content[0]).Designator.Selectors[0];
            Assert.AreEqual(3, nvSelector.Actuals.Count);
            Assert.AreEqual(100, ((NVNumExpression) nvSelector.Actuals[0]).Value);
            Assert.AreEqual(200, ((NVNumExpression) nvSelector.Actuals[1]).Value);
            Assert.AreEqual(300, ((NVNumExpression) nvSelector.Actuals[2]).Value);
        }

        [Test]
        public void ParseReferenceInBetweenXmlTextNodes()
        {
            Parser parser = GetNewParser(
                "text$first.second(100) text");
            //      ^ ^                 ^
            TemplateNode templateNode = parser.ParseTemplate();

            // XmlTextNode
            XmlTextNode text1 = (XmlTextNode)templateNode.GetNodeAt(1, 4);
            AssertPosition(new Position(1, 1, 1, 5), text1.Position);

            // NVReference
            NVDesignator designator = (NVDesignator)templateNode.GetNodeAt(1, 6);
            AssertPosition(new Position(1, 5, 1, 23), designator.Position);

            // XmlTextNode
            XmlTextNode text2 = (XmlTextNode)templateNode.GetNodeAt(1, 24);
            AssertPosition(new Position(1, 23, 1, 28), text2.Position);
        }

        [Test]
        public void ParseReferenceUsingFormalSyntax()
        {
            Parser parser = GetNewParser("${obj.Field}");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference with 1 selector
            NVReference nvReference = (NVReference)templateNode.Content[0];
            Assert.AreEqual("obj", nvReference.Designator.Name);
            Assert.AreEqual("Field", nvReference.Designator.Selectors[0].Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseReferenceWithSilentModifier()
        {
            Parser parser = GetNewParser("$!obj.Field");
            TemplateNode templateNode = parser.ParseTemplate();

            // NVReference with 1 selector
            NVReference nvReference = (NVReference)templateNode.Content[0];
            Assert.AreEqual("obj", nvReference.Designator.Name);
            Assert.AreEqual("Field", nvReference.Designator.Selectors[0].Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParsePartialNVReferenceOnlyWithDollar()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "$");
            TemplateNode template = parser.ParseTemplate();

            // Check errors
            Assert.AreEqual(1, parser.Errors.Count);
            Error error0 = parser.Errors[0];
            Assert.AreEqual("Expected reference identifier", error0.Description);
            AssertPosition(new Position(1, 2, 1, 2), error0.Position);
            Assert.AreEqual(ErrorSeverity.Error, error0.Severity);

            // Check the TemplateNode
            Assert.AreEqual(1, template.Content.Count);
            Assert.AreEqual(typeof(NVReference), template.Content[0].GetType());
        }

        [Test]
        public void ParsePartialNVReferenceWithDollarIdentDot()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "$Ajax.");
            TemplateNode template = parser.ParseTemplate();

            // Check errors
            Assert.AreEqual(1, parser.Errors.Count);
            Error error0 = parser.Errors[0];
            Assert.AreEqual("Expected identifier", error0.Description);
            AssertPosition(new Position(1, 7, 1, 7), error0.Position);

            // Check the TemplateNode
            Assert.AreEqual(1, template.Content.Count);
            NVReference nvReference = (NVReference)template.Content[0];
            Assert.AreEqual("Ajax", nvReference.Designator.Name);
            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);
            AssertPosition(new Position(1, 6, 1, 7), nvReference.Designator.Selectors[0].Position);
        }

        [Test]
        public void ParsePartialNVReferenceWithDollarIdentDotIdentLParen()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions, "$Ajax.LinkToRemote(");
            TemplateNode template = parser.ParseTemplate();

            // Check errors
            Assert.AreEqual(2, parser.Errors.Count);
            AssertError("Expected expression, was 'Error'", new Position(1, 20, 1, 20), parser.Errors[0]);
            AssertError("Expected 'NVRParen' but was 'Error'", new Position(1, 20, 1, 20), parser.Errors[1]);
            
            // Check the TemplateNode
            Assert.AreEqual(1, template.Content.Count);
            NVReference nvReference = (NVReference)template.Content[0];
            Assert.AreEqual("Ajax", nvReference.Designator.Name);

            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);
            NVSelector selector = nvReference.Designator.Selectors[0];
            Assert.AreEqual("LinkToRemote", selector.Name);
            Assert.AreEqual(0, selector.Actuals.Count);
        }

        [Test]
        public void ParsePartialNVReferenceWithDollarIdentDotInXmlElement()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(
                "<tag>\n" +
                "  $Ajax.\n" +
                "</tag>");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check the TemplateNode
            Assert.AreEqual(1, templateNode.Content.Count);
            XmlElement xmlElement = (XmlElement)templateNode.Content[0];
            NVReference nvReference = (NVReference)xmlElement.Content[1];
            Assert.AreEqual("Ajax", nvReference.Designator.Name);
            Assert.AreEqual(1, nvReference.Designator.Selectors.Count);

            AssertPosition(new Position(1, 1, 3, 7), xmlElement.Position);
            AssertPosition(new Position(2, 3, 2, 9), nvReference.Position);
            AssertPosition(new Position(2, 3, 2, 9), nvReference.Designator.Position);
            AssertPosition(new Position(2, 8, 2, 9), nvReference.Designator.Selectors[0].Position);
        }
    }
}