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

namespace Castle.VisualStudio.MonoRailIntelliSenseProvider
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class XmlDocumentationProvider
    {
        private readonly XmlDocument _document;

        public XmlDocumentationProvider(string assemblyFileName)
        {
            string fileName = assemblyFileName.Substring(0, assemblyFileName.Length - 4) + ".xml";

            if (File.Exists(fileName))
            {
                _document = new XmlDocument();
                _document.Load(fileName);
            }
        }

        public string GetTypeDocumentation(string typeFullName)
        {
            if (_document == null)
                return null;

            XmlNode xmlNode = _document.SelectSingleNode(
                string.Format("//member[@name='T:{0}']/summary", typeFullName));
            if (xmlNode != null)
            {
                return RemoveWhitespaceFromSummary(xmlNode.InnerText);
            }

            return null;
        }

        public string GetMethodDocumentation(string typeFullName, string methodName, params string[] parameterTypes)
        {
            if (_document == null)
                return null;

            XmlNodeList xmlNodes = _document.SelectNodes(
                string.Format("//member[contains(@name,'M:{0}.{1}')]/summary", typeFullName, methodName));
            if (xmlNodes.Count > 0)
            {
                return RemoveWhitespaceFromSummary(xmlNodes[0].InnerText);
            }
            return null;
        }

        public string GetParameterDocumentation(string typeFullName, string methodName, string paramName,
            params string[] parameterTypes)
        {
            if (_document == null)
                return null;

            XmlNodeList xmlNodes = _document.SelectNodes(
                string.Format("//member[contains(@name,'M:{0}.{1}')]/param[@name='{2}']",
                typeFullName, methodName, paramName));
            if (xmlNodes.Count > 0)
            {
                return RemoveWhitespaceFromSummary(xmlNodes[0].InnerText);
            }
            return null;
        }

        private static string RemoveWhitespaceFromSummary(string innerText)
        {
            string[] lines = innerText.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!(lines[i].Length == 0 && (i == 0 || i == lines.Length - 1)))
                {
                    output.Append(lines[i].Trim());
                    if (i != lines.Length - 1)
                    {
                        //output.AppendLine();
                        output.Append(" ");
                    }
                }
            }
            return output.ToString();
        }
    }
}