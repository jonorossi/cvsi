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

namespace Castle.NVelocity
{
    using System;
    using Ast;

    public class Position : IComparable<Position>
    {
        private int startLine;
        private int startPos;
        private int endLine;
        private int endPos;

        public Position()
        {
        }

        public Position(int startLine, int startPos)
        {
            this.startLine = startLine;
            this.startPos = startPos;
        }

        public Position(int startLine, int startPos, int endLine, int endPos)
        {
            this.startLine = startLine;
            this.startPos = startPos;
            this.endLine = endLine;
            this.endPos = endPos;
        }

        public Position(Position startAndEndPosition)
        {
            Start = startAndEndPosition;
            End = startAndEndPosition;
        }

        public Position(Position startPosition, Position endPosition)
        {
            Start = startPosition;
            End = endPosition;
        }

        public Position(NVBinaryExpression binExpr)
        {
            if (binExpr.Lhs != null)
                Start = binExpr.Lhs.Position;
            if (binExpr.Rhs != null)
                End = binExpr.Rhs.Position;
        }

        public int StartLine
        {
            get { return startLine; }
            set { startLine = value; }
        }

        //TODO consider renameing *Pos to *Index since it is the index on that line

        public int StartPos
        {
            get { return startPos; }
            set { startPos = value; }
        }

        public Position Start
        {
            get { return new Position(startLine, startPos, startLine, startPos); }
            set
            {
                startLine = value.StartLine;
                startPos = value.StartPos;
            }
        }

        public int EndLine
        {
            get { return endLine; }
            set { endLine = value; }
        }

        public int EndPos
        {
            get { return endPos; }
            set { endPos = value; }
        }

        public Position End
        {
            get { return new Position(endLine, endPos, endLine, endPos); }
            set
            {
                if (value != null)
                    endLine = value.EndLine;
                if (value != null)
                    endPos = value.EndPos;
            }
        }

        public bool Equals(Position anotherPos)
        {
            return (anotherPos.StartLine == startLine) &&
                   (anotherPos.StartPos == startPos) &&
                   (anotherPos.EndLine == endLine) &&
                   (anotherPos.EndPos == endPos);
        }

        public int CompareTo(Position other)
        {
            int lineDiff = startLine - other.StartLine;
            if (lineDiff == 0)
            {
                return startPos - other.StartPos;
            }
            return lineDiff;
        }

        public bool Contains(int line, int pos)
        {
            if (line > startLine && line < endLine)
                return true;

            if (line == startLine && pos >= startPos)
                return true;

            if (line == endLine && pos <= endPos)
                return true;

            return false;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", startLine, startPos, endLine, endPos);
        }
    }
}