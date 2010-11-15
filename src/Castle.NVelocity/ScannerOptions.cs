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

namespace Castle.NVelocity
{
    public class ScannerOptions
    {
        private bool _enableIntelliSenseTriggerTokens;
        private bool _isLineScanner;
        private bool _splitTextTokens;

        /// <summary>
        /// Instructs the scanner to return tokens that form the beginning of a statement
        /// (e.g. '$', '#', '(') as if the expected statement follows. It is used by Castle
        /// Visual Studio Integration so that when a start of statement token is entered
        /// the appropraite IntelliSense can be displayed.
        /// </summary>
        public bool EnableIntelliSenseTriggerTokens
        {
            get { return _enableIntelliSenseTriggerTokens; }
            set { _enableIntelliSenseTriggerTokens = value; }
        }

        /// <summary>
        /// Specifies whether the scanner should return error when reaching the end of the input
        /// with unclosed syntax.
        /// </summary>
        public bool IsLineScanner
        {
            get { return _isLineScanner; }
            set { _isLineScanner = value; }
        }

        /// <summary>
        /// Specifies whether XML and NVelocity text tokens should be returned as multiple
        /// tokens based on word boundaries.
        /// </summary>
        public bool SplitTextTokens
        {
            get { return _splitTextTokens; }
            set { _splitTextTokens = value; }
        }
    }
}