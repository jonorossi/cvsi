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

    public abstract class ScannerTestBase
    {
        protected Scanner _scanner;

        [SetUp]
        public void SetUp()
        {
            _scanner = new Scanner(new ErrorHandler());
        }

        protected void AssertMatchToken(TokenType tokenType)
        {
			AssertMatchToken(_scanner, tokenType);
        }

		protected static void AssertMatchToken(Scanner scanner, TokenType tokenType)
		{
			AssertMatchToken(tokenType, scanner.GetToken());
		}

    	protected static void AssertMatchToken(TokenType tokenType, Token token)
        {
            CheckTokenType(token, tokenType);
        }

		protected void AssertMatchToken(TokenType tokenType, string image)
		{
			AssertMatchToken(_scanner, tokenType, image);
		}

		protected static void AssertMatchToken(Scanner scanner, TokenType tokenType, string image)
		{
			Token token = scanner.GetToken();
			CheckTokenType(token, tokenType);
			CheckImage(token, image);
		}

        protected void AssertMatchToken(TokenType tokenType, string image, Position position)
        {
            Token token = _scanner.GetToken();
            CheckTokenType(token, tokenType);
            CheckImage(token, image);
            CheckPosition(token, position);
        }

        protected static void AssertMatchToken(TokenType tokenType, string image, Token token)
        {
            CheckTokenType(token, tokenType);
            CheckImage(token, image);
        }

        protected void AssertMatchToken(TokenType tokenType, Position position)
        {
            Token token = _scanner.GetToken();
            CheckTokenType(token, tokenType);
            CheckPosition(token, position);
        }

        protected void AssertEOF()
        {
            _scanner.GetToken();
            Assert.IsTrue(_scanner.EOF);
        }

        private static void CheckTokenType(Token token, TokenType tokenType)
        {
            Assert.IsTrue(token.Type == tokenType,
                string.Format("Expected token: '{0}' was '{1}', actual image '{2}'.",
                tokenType, token.Type, token.Image));
        }

        private static void CheckPosition(Token token, Position position)
        {
            Assert.IsNotNull(token.Position, "Position is null.");
            Assert.IsTrue(token.Position.Equals(position),
                string.Format("Expected position: [{0}] was [{1}].", position, token.Position));
        }

        private static void CheckImage(Token token, string image)
        {
            Assert.IsTrue(token.Image == image,
                string.Format("Expected image: '{0}' was '{1}'.", image, token.Image));
        }
    }
}