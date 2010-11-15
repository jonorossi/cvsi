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

namespace Castle.NVelocity.Demo
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using NVelocity;

    class Program
    {
        public static void Main(string[] args)
        {
            Scanner scanner = new Scanner(new ErrorHandler());
            string fileName;

            if (args.Length == 1 && args[0] == "/parser-gui")
            {
                Console.WriteLine("Loading Parser GUI");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ParserGUIForm());
                return;
            }

            Console.WindowHeight = 50;
            Console.WriteLine("Castle.NVelocity Demo");
            Console.WriteLine("=====================");

            if (args.Length > 0)
            {
                fileName = args[0];
            }
            else
            {
                Console.WriteLine("No file specified. Enter file name:");
                fileName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Filename is null, exiting...");
                return;
            }

            Console.WriteLine("Loading: {0}", fileName);
            string source = File.ReadAllText(fileName);

            Console.WriteLine();
            scanner.SetSource(source);

            try
            {
                while (!scanner.EOF)
                {
                    Token token = scanner.GetToken();
                    if (token != null)
                    {
                        switch (token.Type)
                        {
                            case TokenType.XmlText:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case TokenType.XmlComment:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case TokenType.XmlTagName:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case TokenType.XmlTagStart:
                            case TokenType.XmlTagEnd:
                            case TokenType.XmlForwardSlash:
                            case TokenType.XmlEquals:
                            case TokenType.XmlDoubleQuote:
                            case TokenType.XmlQuestionMark:
                            case TokenType.XmlExclaimationMark:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;

                            case TokenType.NVDirectiveHash:
                            case TokenType.NVDirectiveName:
                            case TokenType.NVLParen:
                            case TokenType.NVRParen:
                            case TokenType.NVLBrack:
                            case TokenType.NVRBrack:
                            case TokenType.NVLCurly:
                            case TokenType.NVRCurly:
                            case TokenType.NVDictionaryPercent:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case TokenType.NVTrue:
                            case TokenType.NVFalse:
                            case TokenType.NVIn:
                            case TokenType.NVWith:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case TokenType.NVDollar:
                            case TokenType.NVIdentifier:
                            case TokenType.NVDictionaryKey:
                            case TokenType.NVReferenceLCurly:
                            case TokenType.NVReferenceRCurly:
                            case TokenType.NVReferenceSilent:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case TokenType.NVSingleLineComment:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case TokenType.NVDoubleQuote:
                            case TokenType.NVSingleQuote:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case TokenType.NVStringLiteral:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                        }

                        if (token.Image.EndsWith("\n"))
                            Console.Write(token.Image);
                        else
                            Console.Write(token.Image + " ");

                        Console.ResetColor();
                    }
                }
            }
            catch (ScannerError ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\nScanner Error on line {0}: {1}",
                    scanner.CurrentPos.StartLine, ex.Message);

                File.AppendAllText("ScannerErrors.txt",
                    "=============================================================================="
                    + Environment.NewLine + source + Environment.NewLine);
            }

            Console.ReadLine();
        }
    }
}
