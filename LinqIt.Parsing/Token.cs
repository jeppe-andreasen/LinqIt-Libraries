using System.Linq;
using System.Text;

namespace LinqIt.Parsing
{
    public class Token
    {
        public static readonly char[] WHITE_SPACE = {'\r', '\n', '\t', ' '};
        private readonly string m_Text;
        private int m_Position;

        public Token(string text)
        {
            m_Text = text;
            m_Position = 0;
        }

        public char Current
        {
            get
            {
                if (m_Position < m_Text.Length)
                    return m_Text[m_Position];
                else
                    return '\0';
            }
        }

        public bool IsDone
        {
            get { return m_Position >= m_Text.Length; }
        }

        public string Peek(int length)
        {
            try
            {
                return m_Text.Substring(m_Position, length);
            }
            catch
            {
                if (m_Position < m_Text.Length)
                    return m_Text.Substring(m_Position);
                else
                    return string.Empty;
            }
        }

        public string Peek(params char[] delimiters)
        {
            var result = new StringBuilder();
            var position = m_Position;
            while (position < m_Text.Length && delimiters.Contains(m_Text[position]))
            {
                result.Append(m_Text[position]);
                position++;
            }
            return result.ToString();
        }

        public string Peek(string value)
        {
            var index = m_Text.IndexOf(value, m_Position);
            if (index > -1)
            {
                if (index > m_Position)
                    return m_Text.Substring(m_Position, index - m_Position);
            }
            else if (m_Position < m_Text.Length - 1)
                return m_Text.Substring(m_Position);

            return string.Empty;
        }

        public bool Peeks(string value)
        {
            var peek = Peek(value.Length);
            return string.Compare(peek, value, true) == 0;
        }

        public bool Peeks(params char[] values)
        {
            if (IsDone)
                return false;
            return values.Contains(Current);
        }


        public void MoveToContent()
        {
            while (m_Position < m_Text.Length && WHITE_SPACE.Contains(m_Text[m_Position]))
                m_Position++;
        }

        public void Next()
        {
            m_Position++;
        }

        public void Skip(int characters)
        {
            m_Position += characters;
        }

        public void SkipIfNext(string text, bool ignoreWhiteSpace)
        {
            if (ignoreWhiteSpace)
                MoveToContent();
            if (Peeks(text))
                Skip(text);
        }

        public void Skip(string text)
        {
            m_Position += text.Length;
        }

        public string ReadUntil(params char[] delimiters)
        {
            var result = new StringBuilder();
            while (m_Position < m_Text.Length && !delimiters.Contains(m_Text[m_Position]))
            {
                result.Append(m_Text[m_Position]);
                m_Position++;
            }
            return result.ToString();
        }

        public string ReadUntil(params string[] text)
        {
            if (IsDone)
                return string.Empty;

            var indexes = text.Select(t => m_Text.IndexOf(t, m_Position)).Where(idx => idx >= m_Position);
            if (!indexes.Any())
                return string.Empty;
            var minIndex = indexes.Min();
            var length = minIndex - m_Position;
            if (length == 0)
                return string.Empty;
            var result = m_Text.Substring(m_Position, length);
            m_Position += length;
            return result;
        }

        public string ReadUntilOrEnd(params string[] text)
        {
            var containsText = text.Where(t => m_Text.IndexOf(t, m_Position) > -1).Any();
            if (!containsText)
            {
                var result = m_Text.Substring(m_Position);
                m_Position += result.Length;
                return result;
            }
            else
                return ReadUntil(text);
        }

        public void MovePast(string text)
        {
            ReadUntil(text);
            Skip(text.Length);
        }

        public string ReadString(bool includeDelimiters)
        {
            var delimiter = Current;
            var escape = @"\" + delimiter;
            Skip(1);
            var result = new StringBuilder();
            if (includeDelimiters)
                result.Append(delimiter);
            while (!IsDone)
            {
                if (Peeks(escape))
                {
                    result.Append(delimiter);
                    Skip(escape);
                }
                else if (Peeks(delimiter))
                {
                    Next();
                    break;
                }
                else
                {
                    result.Append(Current);
                    Next();
                }
            }
            if (includeDelimiters)
                result.Append(delimiter);
            return result.ToString();
        }
    }
}