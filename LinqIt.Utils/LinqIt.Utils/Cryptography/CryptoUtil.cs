using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LinqIt.Utils.Cryptography
{
    public class CryptoUtil
    {
        public static string EncryptString(string message, string passPhrase)
        {
            return Transform(message, passPhrase, true);
        }

        public static string DecryptString(string message, string passPhrase)
        {
            return Transform(message, passPhrase, false);
        }

        private static string Transform(string message, string passPhrase, bool encrypt)
        {
            byte[] results;

            var encoding = new System.Text.UTF8Encoding();

            var hashProvider = new MD5CryptoServiceProvider();
            var key = hashProvider.ComputeHash(encoding.GetBytes(passPhrase));

            var cryptoProvider = new TripleDESCryptoServiceProvider();
            cryptoProvider.Key = key;
            cryptoProvider.Mode = CipherMode.ECB;
            cryptoProvider.Padding = PaddingMode.PKCS7;

            var buffer = encrypt ? encoding.GetBytes(message) : Convert.FromBase64String(message);
            try
            {
                var transform = encrypt ? cryptoProvider.CreateEncryptor() : cryptoProvider.CreateDecryptor();
                results = transform.TransformFinalBlock(buffer, 0, buffer.Length);
            }
            finally
            {
                cryptoProvider.Clear();
                hashProvider.Clear();
            }
            return encrypt ? Convert.ToBase64String(results) : encoding.GetString(results);
        }

        public static string Unscramble(string hexstring)
        {
            var builder = new StringBuilder(hexstring.Length / 2);
            for (var i = 0; i <= hexstring.Length - 1; i = i + 2)
                builder.Append((char)int.Parse(hexstring.Substring(i, 2), NumberStyles.HexNumber));
            return builder.ToString();
        }

        public static string Scramble(string text)
        {
            var builder = new StringBuilder(text.Length * 2);
            for (var i = 0; i < text.Length; i++)
            {
                if ((text.Length - (i + 1)) > 0)
                {
                    var tmp = text.Substring(i, 2);
                    switch (tmp)
                    {
                        case @"\n":
                            builder.Append("0A");
                            break;
                        case @"\b":
                            builder.Append("20");
                            break;
                        case @"\r":
                            builder.Append("0D");
                            break;
                        case @"\c":
                            builder.Append("2C");
                            break;
                        case @"\\":
                            builder.Append("5C");
                            break;
                        case @"\0":
                            builder.Append("00");
                            break;
                        case @"\t":
                            builder.Append("07");
                            break;
                        default:
                            builder.Append(String.Format("{0:X2}", (int)(text.ToCharArray())[i]));
                            i--;
                            break;
                    }
                }
                else
                    builder.Append(String.Format("{0:X2}", (int)(text.ToCharArray())[i]));
                i++;
            }
            return builder.ToString();
        }
    }
}
