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
    using Ast;
    using NUnit.Framework;

    [TestFixture]
    public class NVelocityExpressionParserTestCase : ParserTestBase
    {
        [Test]
        public void ParseOrExpression()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "true || false", out parser);

            // Check that LHS and RHS are BooleanExpressions and the operator is Or
            Assert.AreEqual(Operator.Or, expr.Op);
            Assert.AreEqual(true, ((NVBoolExpression)expr.Lhs).Value);
            Assert.AreEqual(false, ((NVBoolExpression)expr.Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseAndExpression()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "true && true and true", out parser);

            // The AST should look like this:
            //    and
            //    / \
            //  and  true
            //  / \
            //true true

            // Check that there is a BinaryExpression with operator 'And'
            Assert.AreEqual(Operator.And, expr.Op);

            // Check the LHS of the expr
            Assert.AreEqual(Operator.And, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Check the RHS of the expr
            Assert.AreEqual(true, ((NVBoolExpression)expr.Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseBinaryExpressionWithRelOp()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 <= 2 && true ne false", out parser);

            // The AST should look like this:
            //     and
            //    /   \
            //  <=     !=
            //  / \    / \
            // 1   2   T  F

            // Check that there is a BinaryExpression with operator 'and'
            Assert.AreEqual(Operator.And, expr.Op);

            // Check that expr.LHS operator is Lte
            Assert.AreEqual(Operator.Lte, ((NVBinaryExpression) expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);
            
            // Check that expr.RHS operator is Neq
            Assert.AreEqual(Operator.Neq, ((NVBinaryExpression)expr.Rhs).Op);
            Assert.AreEqual(true, ((NVBoolExpression)((NVBinaryExpression)expr.Rhs).Lhs).Value);
            Assert.AreEqual(false, ((NVBoolExpression)((NVBinaryExpression)expr.Rhs).Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseBinaryExpressionWithAddOp()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 + 2 - 3", out parser);

            // The AST should look like this:
            //     -
            //    / \
            //   +   3
            //  / \
            // 1   2

            // Check that there is a BinaryExpression with operator 'Minus'
            Assert.AreEqual(Operator.Minus, expr.Op);

            // Check the LHS of the expr
            Assert.AreEqual(Operator.Plus, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Check the RHS of the expr
            Assert.AreEqual(3, ((NVNumExpression)expr.Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseBinaryExpressionWithMulOp()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 * 2 / 3", out parser);

            // Check that there is a BinaryExpression with operator 'Div'
            Assert.AreEqual(Operator.Div, expr.Op);

            // Check the LHS
            Assert.AreEqual(Operator.Mul, ((NVBinaryExpression)expr.Lhs).Op);
            Assert.AreEqual(1, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Lhs).Value);
            Assert.AreEqual(2, ((NVNumExpression)((NVBinaryExpression)expr.Lhs).Rhs).Value);

            // Chec the RHS
            Assert.AreEqual(3, ((NVNumExpression)expr.Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseUnaryExpression()
        {
            Parser parser;

            GetExpressionFromTemplate("+1 * -1", out parser);
            //TODO: Check UnaryExpression objects
            AssertNoErrors(parser);

            GetExpressionFromTemplate("!false and !!true", out parser);
            //TODO: Check UnaryExpression objects
            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNVReference()
        {
            Parser parser;

            GetExpressionFromTemplate("$obj.Field", out parser);
            //TODO: Check DesignatorExpression
            AssertNoErrors(parser);

            GetExpressionFromTemplate("$obj.Method($obj2)", out parser);
            //TODO: Check DesignatorExpression
            AssertNoErrors(parser);
        }

        [Test]
        public void ParseExpressionWithParentheses()
        {
            Parser parser;
            NVBinaryExpression expr = (NVBinaryExpression)GetExpressionFromTemplate(
                "1 * (2 + 3)", out parser);

            // Check expr
            Assert.AreEqual(Operator.Mul, expr.Op);

            // Check LHS
            Assert.AreEqual(1, ((NVNumExpression)expr.Lhs).Value);
            
            // Check RHS
            NVBinaryExpression rhs = (NVBinaryExpression)expr.Rhs;
            Assert.AreEqual(Operator.Plus, rhs.Op);
            Assert.AreEqual(2, ((NVNumExpression)rhs.Lhs).Value);
            Assert.AreEqual(3, ((NVNumExpression)rhs.Rhs).Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseBooleanExpression()
        {
            Parser parser;

            NVBoolExpression exprTrue = (NVBoolExpression)GetExpressionFromTemplate("true", out parser);
            Assert.AreEqual(true, exprTrue.Value);
            AssertNoErrors(parser);

            NVBoolExpression exprFalse = (NVBoolExpression)GetExpressionFromTemplate("false", out parser);
            Assert.AreEqual(false, exprFalse.Value);
            AssertNoErrors(parser);
        }

        [Test]
        public void ParseNumExpression()
        {
            Parser parser;
            NVNumExpression expr = (NVNumExpression)GetExpressionFromTemplate("100", out parser);

            // Check that the value of the expression is '100'
            Assert.AreEqual(100, expr.Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseStringLiteralSingleQuotes()
        {
            Parser parser;
            NVStringExpression expr = (NVStringExpression)GetExpressionFromTemplate("'string'", out parser);

            // Check that the value of the expression is 'string'
            Assert.AreEqual("string", expr.Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseStringLiteralDoubleQuotes()
        {
            Parser parser;
            NVStringExpression expr = (NVStringExpression)GetExpressionFromTemplate("\"string\"", out parser);

            // Check that the value of the expression is 'string'
            Assert.AreEqual("string", expr.Value);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseDictionaryEmpty()
        {
            Parser parser;
            GetExpressionFromTemplate("\"%{}\"", out parser);
            //TODO: Check NVDictionary

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseDictionaryWithOnePair()
        {
            Parser parser;
            GetExpressionFromTemplate("\"%{ key='value' }\"", out parser);
            //TODO: Check NVDictionary

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseDictionaryWithTwoPairs()
        {
            Parser parser;
            GetExpressionFromTemplate("\"%{ key='value', anotherKey='anotherValue' }\"", out parser);
            //TODO: Check NVDictionary

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseDictionaryWithReferenceAsValue()
        {
            Parser parser;
            GetExpressionFromTemplate("\"%{ key=$var.Field }\"", out parser);
            //TODO: Check NVDictionary

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseRangeWithTwoConstants()
        {
            Parser parser;
            GetExpressionFromTemplate("[1..10]", out parser);
            //TODO

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseRangeWithConstantAndReference()
        {
            Parser parser;
            GetExpressionFromTemplate("[1..$n]", out parser);
            //TODO

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseRangeWithTwoReferences()
        {
            Parser parser;
            GetExpressionFromTemplate("[$i..$n]", out parser);
            //TODO

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseArrayEmpty()
        {
            Parser parser;
            GetExpressionFromTemplate("[]", out parser);
            //TODO

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseArrayWithSingleExpression()
        {
            Parser parser;
            GetExpressionFromTemplate("[10]", out parser);
            //TODO

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseArrayWithTwoExpressions()
        {
            Parser parser;
            GetExpressionFromTemplate("[10, 20]", out parser);

            AssertNoErrors(parser);
        }

        [Test]
        public void ParseArrayWithVariousDifferentExpressions()
        {
            Parser parser;
            GetExpressionFromTemplate("[\"MonoRail\", $is, \"cool\"]", out parser);
            //TODO

            AssertNoErrors(parser);
        }
    }
}