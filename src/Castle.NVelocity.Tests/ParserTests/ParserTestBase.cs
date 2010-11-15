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

    public abstract class ParserTestBase
    {
        protected static Parser GetNewParser(string template)
        {
            return GetNewParser(new ScannerOptions(), template);
        }

        protected static Parser GetNewParser(ScannerOptions scannerOptions, string template)
        {
            Scanner scanner = new Scanner(new ErrorHandler());
            scanner.Options = scannerOptions;
            scanner.SetSource(template);

            return new Parser(scanner, new ErrorHandler());
        }

        protected static void AssertPosition(Position expected, Position actual)
        {
            Assert.IsNotNull(actual, "Position is null.");
            Assert.IsTrue(actual.Equals(expected),
                string.Format("Expected position: [{0}] was [{1}].", expected, actual));
        }

        protected static void AssertError(string expectedDescription, Position expectedPosition, Error actual)
        {
            Assert.AreEqual(expectedDescription, actual.Description);
            AssertPosition(expectedPosition, actual.Position);
        }

        protected static NVExpression GetExpressionFromTemplate(string template, out Parser parser)
        {
            // Create Parser
            parser = GetNewParser(string.Format("$var.Method({0})", template));

            // Parse Expression
            TemplateNode templateNode = parser.ParseTemplate();

            AssertNoErrors(parser);

            // Return the first parameter in the method call
            return ((NVReference)templateNode.Content[0]).Designator.Selectors[0].Actuals[0];
        }

        protected static void AssertPositionForExpression(Position expected, Position actual)
        {
            const int leftOffset = 12; // Not 13 because StartPos is 1-based
            
            AssertPosition(new Position(expected.StartLine, expected.StartPos + leftOffset,
                expected.EndLine, expected.EndPos + leftOffset), actual);
        }

        protected static void AssertNoErrors(Parser parser)
        {
            // Assert there are no errors
            if (parser.Errors.Count > 0)
            {
                string message = "There are parsing errors.";
                for (int i = 0; i < parser.Errors.Count; i++)
                {
                    message += string.Format("\nError {0}: \"{1}\" at {2}", i, parser.Errors[0].Description, parser.Errors[0].Position);
                }

                Assert.AreEqual(0, parser.Errors.Count, message);
            }
        }
    }
}