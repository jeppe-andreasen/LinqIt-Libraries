using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax.Parsing
{
    public class Token
    {
        // Fields
        private int m_Position;
        private string m_Text;
        public static readonly char[] WHITE_SPACE = new char[] { '\r', '\n', '\t', ' ' };

        // Methods
        public Token(string text)
        {
            this.m_Text = text;
            this.m_Position = 0;
        }

        public void MovePast(string text)
        {
            this.ReadUntil(new string[] { text });
            this.Skip(text.Length);
        }

        public void MoveToContent()
        {
            while ((this.m_Position < this.m_Text.Length) && WHITE_SPACE.Contains<char>(this.m_Text[this.m_Position]))
            {
                this.m_Position++;
            }
        }

        public void Next()
        {
            this.m_Position++;
        }

        public string Peek(params char[] delimiters)
        {
            StringBuilder result = new StringBuilder();
            for (int position = this.m_Position; (position < this.m_Text.Length) && delimiters.Contains<char>(this.m_Text[position]); position++)
            {
                result.Append(this.m_Text[position]);
            }
            return result.ToString();
        }

        public string Peek(string value)
        {
            int index = this.m_Text.IndexOf(value, this.m_Position);
            if (index > -1)
            {
                if (index > this.m_Position)
                {
                    return this.m_Text.Substring(this.m_Position, index - this.m_Position);
                }
            }
            else if (this.m_Position < (this.m_Text.Length - 1))
            {
                return this.m_Text.Substring(this.m_Position);
            }
            return string.Empty;
        }

        public string Peek(int length)
        {
            try
            {
                return this.m_Text.Substring(this.m_Position, length);
            }
            catch
            {
                if (this.m_Position < this.m_Text.Length)
                {
                    return this.m_Text.Substring(this.m_Position);
                }
                return string.Empty;
            }
        }

        public bool Peeks(string value)
        {
            return (string.Compare(this.Peek(value.Length), value, true) == 0);
        }

        public string ReadUntil(params char[] delimiters)
        {
            StringBuilder result = new StringBuilder();
            while ((this.m_Position < this.m_Text.Length) && !delimiters.Contains<char>(this.m_Text[this.m_Position]))
            {
                result.Append(this.m_Text[this.m_Position]);
                this.m_Position++;
            }
            return result.ToString();
        }

        public string ReadUntil(params string[] text)
        {
            if (this.IsDone)
            {
                return string.Empty;
            }
            IEnumerable<int> indexes = text.Select<string, int>(delegate(string t)
            {
                return this.m_Text.IndexOf(t, this.m_Position);
            }).Where<int>(delegate(int idx)
            {
                return idx >= this.m_Position;
            });
            if (!indexes.Any<int>())
            {
                return string.Empty;
            }
            int length = indexes.Min() - this.m_Position;
            if (length == 0)
            {
                return string.Empty;
            }
            string result = this.m_Text.Substring(this.m_Position, length);
            this.m_Position += length;
            return result;
        }

        public string ReadUntilOrEnd(params string[] text)
        {
            if (!text.Where<string>(delegate(string t)
            {
                return (this.m_Text.IndexOf(t, this.m_Position) > -1);
            }).Any<string>())
            {
                string result = this.m_Text.Substring(this.m_Position);
                this.m_Position += result.Length;
                return result;
            }
            return this.ReadUntil(text);
        }

        public void Skip(int characters)
        {
            this.m_Position += characters;
        }

        public void Skip(string text)
        {
            this.m_Position += text.Length;
        }

        // Properties
        public char Current
        {
            get
            {
                if (this.m_Position < this.m_Text.Length)
                {
                    return this.m_Text[this.m_Position];
                }
                return '\0';
            }
        }

        public bool IsDone
        {
            get
            {
                return (this.m_Position >= this.m_Text.Length);
            }
        }
    }
}
