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
	using System.Collections.Generic;
	using System.Xml;

	public class XhtmlSchemaProvider
	{
		private readonly XmlDocument _doc = new XmlDocument();
		private readonly XmlNamespaceManager _nsMgr;

		public XhtmlSchemaProvider(string fileName)
		{
			_doc.Load(fileName);

			_nsMgr = new XmlNamespaceManager(_doc.NameTable);
			_nsMgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
		}

		public List<string> GetElements()
		{
			List<string> xhtmlElements = new List<string>();
			foreach (XmlElement element in _doc.SelectNodes("xs:schema/xs:element", _nsMgr))
			{
				xhtmlElements.Add(element.GetAttribute("name"));
			}
			xhtmlElements.Sort();
			return xhtmlElements;
		}

		public List<string> GetAttributes(string elementName)
		{
			List<string> xhtmlAttributes = new List<string>();
			foreach (XmlElement element in _doc.SelectNodes(string.Format(
				"xs:schema/xs:element[@name='{0}']/xs:complexType/xs:attribute", elementName), _nsMgr))
			{
				xhtmlAttributes.Add(element.GetAttribute("name"));
			}
			xhtmlAttributes.Sort();
			return xhtmlAttributes;
		}
	}
}