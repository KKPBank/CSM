using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

///<summary>
/// Class Name : StringHelpers
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.Common.Utilities
{
    public static class StringHelpers
    {
        public static string ConvertByteToString(byte[] byteArray)
        {
            Encoding encoding = Encoding.UTF8;
            string str = encoding.GetString(byteArray);
            return str;
        }

        public static IList<string> ConvertStringToList(string str)
        {
            List<string> namesList = null;

            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(','))
                {
                    string[] namesArray = str.Split(',').Select(s => s.Trim()).ToArray();
                    namesList = new List<string>(namesArray.Length);
                    namesList.AddRange(namesArray);
                }
                else
                {
                    namesList = new List<string>(1);
                    namesList.Add(str);
                }
            }

            return namesList;
        }

        public static IList<object> ConvertStringToList(string str, char seperator)
        {
            List<object> namesList = null;

            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains(seperator))
                {
                    string[] namesArray = str.Split(seperator).Select(s => s.Trim()).ToArray();
                    namesList = new List<object>(namesArray.Length);
                    namesList.AddRange(namesArray);
                }
                else
                {
                    namesList = new List<object>(1);
                    namesList.Add(str);
                }
            }

            return namesList;
        }

        public static string ConvertListToString(List<object> list, string seperator)
        {
            object[] arr = null;
            string str = string.Empty;

            if (list != null && list.Count > 0)
            {
                arr = list.Where(x => !string.IsNullOrWhiteSpace(x.ConvertToString())).ToArray();
                str = string.Join(seperator, arr);
            }

            return str;
        }

        public static string ConvertListToString(IList<object> list, string seperator)
        {
            object[] arr = null;
            string str = string.Empty;

            if (list != null && list.Count > 0)
            {
                arr = list.ToArray();
                str = string.Join(seperator, arr);
            }

            return str;
        }

        public static string Substring(byte[] byteArray)
        {
            string str = ConvertByteToString(byteArray);
            return str.Substring(0, Math.Min(str.Length, 20));
        }

        public static string Substring(string value, string str)
        {
            string result = string.Empty;

            int startIndex = value.IndexOf(str) + str.Length;
            int endIndex = value.IndexOf(";", startIndex);
            int length = endIndex - startIndex;

            if (startIndex > 0)
            {
                result = value.Substring(startIndex, length);
            }

            return result;
        }

        public static bool OnlyNumberInString(string test)
        {
            return Regex.IsMatch(test, @"\A\b[0-9]+\b\Z");
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            var dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static string ConcatAll(string[] arr, string pattern, string delimiter)
        {
            string res = string.Empty;

            if (arr != null && arr.Length > 0)
            {
                res = arr.Where(o => !string.IsNullOrWhiteSpace(o))
                    .Aggregate((current, next) => current + delimiter + string.Format(pattern, next));
            }

            return string.Format(pattern, res);
        }

        public static string ConvertToString(this object d)
        {
            string result = string.Empty;

            if (d != null)
            {
                result = d.ToString();
            }

            return result;
        }

        public static string NullSafeTrim(this string value)
        {
            string str = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                str = value.Trim();
            }

            return str;
        }

        public static string ReplaceEmptyLine(this string value)
        {
            string str = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                str = value.Replace(Environment.NewLine, "\n");
            }

            return str;
        }

        public static string ToLineBreak(this string value)
        {
            string str = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                str = value.Replace(Environment.NewLine, "<br/>");
            }

            return str;
        }

        public static string ConvertToUTF8(this string s)
        {
            //string to utf
            byte[] utf = Encoding.UTF8.GetBytes(s);

            //utf to string
            return Encoding.UTF8.GetString(utf);
        }

        public static string DisplayWithMaxLength(string original)
        {
            return (string.IsNullOrEmpty(original) ? "" : ((original.Length > Constants.DisplayMaxLength) ? (original.Substring(0, Constants.DisplayMaxLength) + "...") : original));
        }

        public static string DisplayLimitLength(string original)
        {
            return (string.IsNullOrEmpty(original) ? "" : ((original.Length > Constants.DisplayMaxLength) ? (original.Substring(0, Constants.DisplayMaxLength)) : original));
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidEmails(string emails)
        {
            try
            {
                var emailList = emails.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in emailList)
                {
                    if (!IsValidEmail(item))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CaseInsensitiveComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}