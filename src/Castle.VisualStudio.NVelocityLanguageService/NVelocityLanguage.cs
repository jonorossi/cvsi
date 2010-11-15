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

namespace Castle.VisualStudio.NVelocityLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Castle.NVelocity;
    using Castle.NVelocity.Ast;
    using Castle.VisualStudio.MonoRailIntelliSenseProvider;
#if DEBUG
    using Castle.VisualStudio.NVelocityLanguageService.DebugWindow;
#endif
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;
    using ErrorHandler = Castle.NVelocity.ErrorHandler;

    [Guid(NVelocityConstants.LanguageServiceGuidString)]
    public class NVelocityLanguage : LanguageService
    {
        private LanguagePreferences preferences;
        private readonly ColorableItem[] _colorableItems;

#if DEBUG
        private readonly DebugForm _debugForm;
#endif

        private TemplateNode _templateNode;

        public NVelocityLanguage()
        {
            #region Colorable Items

            _colorableItems = new ColorableItem[] {
                // NVText
                new ColorableItem("NVelocity – Text", "NVelocity – Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVKeyword
                new ColorableItem("NVelocity – Keyword", "NVelocity – Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_BOLD),
                // NVComment
                new ColorableItem("NVelocity – Comment", "NVelocity – Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVIdentifier
                new ColorableItem("NVelocity – Identifier", "NVelocity – Identifier", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVString
                new ColorableItem("NVelocity – String", "NVelocity – String", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVNumber
                new ColorableItem("NVelocity – Number", "NVelocity – Number", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDirective
                new ColorableItem("NVelocity – Directive", "NVelocity – Directive", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_BOLD),
                // NVOperator
                new ColorableItem("NVelocity – Operator", "NVelocity – Operator", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVBracket
                new ColorableItem("NVelocity – Bracket", "NVelocity – Bracket", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryDelimiter
                new ColorableItem("NVelocity – Dictionary Delimiter", "NVelocity – Dictionary Delimiter", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryKey
                new ColorableItem("NVelocity – Dictionary Key", "NVelocity – Dictionary Key", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // NVDictionaryEquals
                new ColorableItem("NVelocity – Dictionary Equals", "NVelocity – Dictionary Equals", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlText
                new ColorableItem("NVelocity – XML Text", "NVelocity – XML Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlComment
                new ColorableItem("NVelocity – XML Comment", "NVelocity – XML Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlTagName
                new ColorableItem("NVelocity – XML Tag Name", "NVelocity – XML Tag Name", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlAttributeName
                new ColorableItem("NVelocity – XML Attribute Name", "NVelocity – XML Attribute Name", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlAttributeValue
                new ColorableItem("NVelocity – XML Attribute Value", "NVelocity – XML Attribute Value", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlTagDelimiter
                new ColorableItem("NVelocity – XML Tag Delimiter", "NVelocity – XML Tag Delimiter", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlOperator
                new ColorableItem("NVelocity – XML Operator", "NVelocity – XML Operator", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlEntity
                new ColorableItem("NVelocity – XML Entity", "NVelocity – XML Entity", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                // XmlCDataSection
                new ColorableItem("NVelocity – XML CData Section", "NVelocity – XML CData Section", COLORINDEX.CI_DARKGRAY, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT)
                // XmlProcessingInstruction
                // ===== not done
            };
            #endregion

#if DEBUG
            _debugForm = new DebugForm();
            _debugForm.Show();
#endif
        }

        public override void Dispose()
        {
            try
            {
                // Dispose the preferences
                if (preferences != null)
                {
                    preferences.Dispose();
                    preferences = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (preferences == null)
            {
                preferences = new LanguagePreferences(Site, typeof(NVelocityLanguage).GUID, Name);
                preferences.Init();
                preferences.LineNumbers = true;
                preferences.Apply();
            }
            return preferences;
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new Source(this, buffer, GetColorizer(buffer));
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            // Return a new scanner for every file because Visual Studio calls this method
            // for every file that is opened.
            return new NVelocityScanner();
        }

        public override string Name
        {
            get { return "NVelocity"; }
        }

        public override void OnIdle(bool periodic)
        {
            Source src = GetSource(LastActiveTextView);
            if (src != null && src.LastParseTime == Int32.MaxValue)
            {
                src.LastParseTime = 0;
            }
            base.OnIdle(periodic);
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req");
            }

            Trace.WriteLine(string.Format("NVelocityLanguage.ParseSource(). Reason:{0}", req.Reason));

            // Parse the input if required
            if (req.Reason == ParseReason.Check ||
                req.Reason == ParseReason.DisplayMemberList ||
                req.Reason == ParseReason.MemberSelect ||
                req.Reason == ParseReason.MethodTip)
            {
                ErrorHandler errors = new ErrorHandler();

                try
                {
                    Scanner scanner = new Scanner(errors);
                    scanner.Options.EnableIntelliSenseTriggerTokens = true;
                    scanner.SetSource(req.Text);

                    Parser parser = new Parser(scanner, errors);

                    _templateNode = parser.ParseTemplate();

                    // Prepare the template node so that all the helpers are available
                    PrepareTemplateNode(req.FileName);

                    _templateNode.DoSemanticChecks(errors);
                }
                catch (ScannerError se)
                {
                    req.Sink.AddError(req.FileName, "Scanner Error: " + se.Message, new TextSpan(),
                        Severity.Error);
                }
                catch (Exception ex)
                {
                    req.Sink.AddError(req.FileName, "FATAL: " + ex, new TextSpan(), Severity.Error);
                }
                finally
                {
                    for (int i = 0; i < errors.Count; i++)
                    {
                        Error error = errors[i];

                        TextSpan textSpan = new TextSpan();
                        textSpan.iStartLine = error.Position.StartLine - 1;
                        textSpan.iStartIndex = error.Position.StartPos - 1;
                        textSpan.iEndLine = error.Position.EndLine - 1;
                        textSpan.iEndIndex = error.Position.EndPos - 1;

                        Severity severity = Severity.Fatal;
                        if (error.Severity == ErrorSeverity.Error)
                            severity = Severity.Error;
                        else if (error.Severity == ErrorSeverity.Warning)
                            severity = Severity.Warning;
                        else if (error.Severity == ErrorSeverity.Message)
                            severity = Severity.Hint;

                        req.Sink.AddError(req.FileName, error.Description, textSpan, severity);
                    }
                }

#if DEBUG
                int line;
                int column;
                req.View.GetCaretPos(out line, out column);
                _debugForm.UpdateUI(line + 1, column + 1, _templateNode);
#endif
            }
            else
            {
                //MessageBox.Show("Unparsed ParseReason: " + req.Reason);
            }

            // Perform other operations
            if (req.Reason == ParseReason.MethodTip)
            {
                TextSpan textSpan = new TextSpan();
                textSpan.iStartLine = req.Line;
                textSpan.iStartIndex = req.Col - 1;
                textSpan.iEndLine = req.Line;
                textSpan.iEndIndex = req.Col;

                req.Sink.StartName(textSpan, "");
                req.Sink.StartParameters(textSpan);

                Trace.WriteLine("MethodTip at line " + req.Line + " col " + req.Col);
            }

            if (req.Sink.HiddenRegions)
            {
                AddHiddenRegions(req.Sink, _templateNode.Content);

                req.Sink.ProcessHiddenRegions = true;
            }

            NVelocityAuthoringScope scope = new NVelocityAuthoringScope(_templateNode, req.FileName);

            //if (req.Sink.BraceMatching && req.Col > 30)
            //{
            //    TextSpan startBrace = new TextSpan();
            //    startBrace.iStartLine = req.Line;
            //    startBrace.iStartIndex = 20;
            //    startBrace.iEndLine = req.Line;
            //    startBrace.iEndIndex = 21;

            //    TextSpan endBrace = new TextSpan();
            //    endBrace.iStartLine = req.Line;
            //    endBrace.iStartIndex = req.Col - 1;
            //    endBrace.iEndLine = req.Line;
            //    endBrace.iEndIndex = req.Col;

            //    req.Sink.MatchPair(startBrace, endBrace, 0);
            //}

            return scope;
        }

        private static void AddHiddenRegions(AuthoringSink sink, IEnumerable<AstNode> content)
        {
            foreach (AstNode astNode in content)
            {
                if ((astNode is XmlElement || astNode is NVForeachDirective)
                    && (astNode.Position.StartLine < astNode.Position.EndLine))
                {
//                    NewHiddenRegion region = new NewHiddenRegion();
//                    region.dwBehavior = (uint)HIDDEN_REGION_BEHAVIOR.hrbEditorControlled;
//                    region.dwState = (uint)HIDDEN_REGION_STATE.hrsExpanded;
//                    region.iType = (int)HIDDEN_REGION_TYPE.hrtCollapsible;
//                    region.pszBanner = "..." + xmlElement.Name + "...";

                    TextSpan hiddenTextSpan = new TextSpan();
                    hiddenTextSpan.iStartLine = astNode.Position.StartLine - 1;
                    hiddenTextSpan.iStartIndex = astNode.Position.StartPos - 1;
                    hiddenTextSpan.iEndLine = astNode.Position.EndLine - 1;
                    hiddenTextSpan.iEndIndex = astNode.Position.EndPos - 1;
//                    region.tsHiddenText = hiddenTextSpan;

                    sink.AddHiddenRegion(hiddenTextSpan);

                    // Add child regions
                    if (astNode is XmlElement)
                        AddHiddenRegions(sink, ((XmlElement)astNode).Content);
                    else if (astNode is NVForeachDirective)
                        AddHiddenRegions(sink, ((NVForeachDirective)astNode).Content);
                }
            }
        }

        private void PrepareTemplateNode(string fileName)
        {
            string binDirectory = IntelliSenseProvider.FindBinaryDirectory(fileName);
            if (string.IsNullOrEmpty(binDirectory))
            {
                return;
            }

            IntelliSenseProvider intelliSenseProvider = new IntelliSenseProvider(binDirectory);

            // Get all available helpers and add them to the template node
            foreach (NVClassNode classNode in intelliSenseProvider.GetHelpers())
            {
                _templateNode.AddClass(classNode);

                // Add a localnode/variable to the scope for each helper
                string varName = classNode.Name;
                if (classNode.Name.EndsWith("Helper"))
                {
                    varName = varName.Substring(0, varName.Length - 6);
                }
                _templateNode.AddVariable(new NVLocalNode(varName, classNode));
            }

            // Get all available view components and add them to the template node
            foreach (NVClassNode viewComponentClassNode in intelliSenseProvider.GetViewComponents())
            {
                _templateNode.AddClass(viewComponentClassNode);
            }
        }

        public override int GetItemCount(out int count)
        {
            count = _colorableItems.Length;
            return VSConstants.S_OK;
        }

        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1 || index > _colorableItems.Length)
            {
                item = null;
                return VSConstants.S_FALSE;
            }
            item = _colorableItems[index - 1];
            return VSConstants.S_OK;
        }

#if !VS2005
        public override string GetFormatFilterList()
        {
            return "";
        }
#endif
    }
}