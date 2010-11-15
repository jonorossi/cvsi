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
    using System.Collections.Generic;

    public enum ErrorSeverity { Error, Warning, Message }

    public class ErrorHandler
    {
        private readonly List<Error> _errors = new List<Error>();

        public void AddError(string description, Position position, ErrorSeverity severity)
        {
            if (position == null)
            {
                position = new Position(1, 1);
            }
            
            _errors.Add(new Error(description, position, severity));
        }

        public int Count
        {
            get { return _errors.Count; }
        }

        public Error this[int index]
        {
            get { return _errors[index]; }
        }
    }

    public class Error
    {
        private string _description;
        private Position _position;
        private ErrorSeverity _severity;

        public Error(string description, Position position, ErrorSeverity severity)
        {
            _description = description;
            _position = position;
            _severity = severity;
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public ErrorSeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }
    }
}