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

using System;

namespace Castle.NVelocity.Tests.ParserTests
{
    using Castle.NVelocity.Ast;
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityDirectiveTestCase : ParserTestBase
    {
        [Test]
        public void ParseComponentWithNoParameters()
        {
            Parser parser = GetNewParser("#component()");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check NVDirective
            Assert.AreEqual(1, templateNode.Content.Count);
            NVDirective nvDirective = (NVDirective)templateNode.Content[0];
            Assert.AreEqual("component", nvDirective.Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseSetDirectiveSimpleNumExpression()
        {
            Parser parser = GetNewParser("#set($x = 10)");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check NVDirective
            NVDirective nvDirective = (NVDirective)templateNode.Content[0];
            Assert.AreEqual("set", nvDirective.Name);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseForeachDirective()
        {
            Parser parser = GetNewParser(
                "#foreach($item in $collection)text here#end text");
            TemplateNode templateNode = parser.ParseTemplate();

            // Do semantic checks so the scope is populated
            templateNode.DoSemanticChecks(new ErrorHandler());

            // Check Template
            Assert.AreEqual(2, templateNode.Content.Count);
            Assert.AreEqual(" text", ((XmlTextNode)templateNode.Content[1]).Text);

            // Check NVForeachDirective
            NVForeachDirective foreachDirective = (NVForeachDirective)templateNode.Content[0];
            AssertPosition(new Position(1, 1, 1, 44), foreachDirective.Position);
            Assert.AreEqual("item", foreachDirective.Iterator);
            //TODO: Assert.AreEqual("collection", ((NVDesignatorExpression)foreachDirective.Collection).Designator.Name);

            // Check directive content
            Assert.AreEqual(1, foreachDirective.Content.Count);
            Assert.AreEqual("text here", ((XmlTextNode)foreachDirective.Content[0]).Text);

            // Check iterator variable is only in the foreach scope
            Assert.IsFalse(templateNode.Scope.Exists("item"));
            Assert.IsTrue(foreachDirective.Scope.Exists("item"));

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseForeachDirectiveWithStartOfReference()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions,
                "text #foreach($item in $collection) $  #end text");
            TemplateNode templateNode = parser.ParseTemplate();

            // Check Position, it should get to just after the '$'
            AssertPosition(new Position(1, 6, 1, 38), templateNode.Content[1].Position);

            Assert.AreEqual(2, parser.Errors.Count);
        }

        [Test]
        public void ParseDirectiveStartWithNoNameFollowedByNewLine()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions,
                "#" + Environment.NewLine);
            TemplateNode templateNode = parser.ParseTemplate();

            // Ensure the whole directive is included
            AssertPosition(new Position(1, 1, 2, 2), templateNode.GetNodeAt(1, 1).Position);

            Assert.AreEqual(1, parser.Errors.Count);
            AssertError("Expected directive name", new Position(2, 2, 2, 2), parser.Errors[0]);
        }

        [Test]
        public void ParseDirectiveStartWithNoNameFollowedByWhitespace()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;

            Parser parser = GetNewParser(scannerOptions,
                "# ");
            TemplateNode templateNode = parser.ParseTemplate();

            // Ensure the whole directive is included
            AssertPosition(new Position(1, 1, 1, 3), templateNode.GetNodeAt(1, 1).Position);

            Assert.AreEqual(1, parser.Errors.Count);
            AssertError("Expected directive name", new Position(1, 3, 1, 3), parser.Errors[0]);
        }

        [Test]
        public void IncompleteDirectiveFollowedByAnotherDirectiveBreaksOutOfFirstDirectiveParams()
        {
            ScannerOptions scannerOptions = new ScannerOptions();
            scannerOptions.EnableIntelliSenseTriggerTokens = true;
            Parser parser = GetNewParser(scannerOptions,
                "#component(\n" +
                "#component(MyComponent)");
            TemplateNode templateNode = parser.ParseTemplate();

            // Ensure that both directives have been parsed as separate directives
            Assert.AreEqual(2, templateNode.Content.Count);
            Assert.IsInstanceOfType(typeof(NVDirective), templateNode.Content[0]);
            Assert.IsInstanceOfType(typeof(NVDirective), templateNode.Content[1]);

            // Check the error about the incomplete directive
            Assert.AreEqual(1, parser.Errors.Count);
            AssertError("Incomplete directive.", new Position(2, 2, 2, 2), parser.Errors[0]);
        }
    }
}