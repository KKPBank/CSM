using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using CSM.Common.Resources;
using CSM.Common.Securities;
using HtmlAgilityPack;
using log4net;
using System.ComponentModel.DataAnnotations;

namespace CSM.Common.Utilities
{
    public static class ApplicationHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ApplicationHelpers));
        private static readonly ResourceManager rm = new ResourceManager("CSM.Common.Resources.Resource", typeof(Resource).Assembly);

        public static string GetMessage(string resourceKey)
        {
            string message;

            try
            {
                message = rm.GetString(resourceKey);
            }
            catch
            {
                message = "Could not find global resource.";
                Logger.DebugFormat("{0}, Could not find global resource.", resourceKey);
            }

            return message;
        }

        public static string GetMessage(string resourceKey, string cultureName)
        {
            string message;

            try
            {
                message = rm.GetString(resourceKey, GetCultureInfo(cultureName));
            }
            catch
            {
                message = "Could not find global resource.";
                Logger.DebugFormat("{0}, Could not find global resource.", resourceKey);
            }

            return message;
        }

        /// <summary>
        ///     Gets the client's IP address.
        ///     This method takes into account the X-Forwarded-For header,
        ///     in case the blog is hosted behind a load balancer or proxy.
        /// </summary>
        /// <returns>The client's IP address.</returns>
        public static string GetClientIP()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpRequest request = context.Request;
                if (request != null)
                {
                    try
                    {
                        string userHostAddress = request.UserHostAddress;

                        // Attempt to parse.  If it fails, we catch below and return "0.0.0.0"
                        // Could use TryParse instead, but I wanted to catch all exceptions
                        IPAddress.Parse(userHostAddress);

                        string xForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                        if (string.IsNullOrEmpty(xForwardedFor))
                            return userHostAddress;

                        // Get a list of public ip addresses in the X_FORWARDED_FOR variable
                        List<string> publicForwardingIps =
                            xForwardedFor.Split(',').Where(ip => !IsPrivateIpAddress(ip)).ToList();

                        // If we found any, return the last one, otherwise return the user host address
                        return publicForwardingIps.Any() ? publicForwardingIps.Last() : userHostAddress;
                    }
                    catch (Exception)
                    {
                        // Always return all zeroes for any failure (my calling code expects it)
                        //return "0.0.0.0";
                        return WebConfig.GetDefaultIP();
                    }
                }
            }

            //return "0.0.0.0";
            return WebConfig.GetDefaultIP();
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            IPAddress ip = IPAddress.Parse(ipAddress);
            byte[] octets = ip.GetAddressBytes();

            bool is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            bool is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            bool is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            bool isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        public static string GenerateRefNo()
        {
            String timeStamp = DateTime.Now.FormatDateTime("yyyyMMddfffff");
            return string.Format("{0}", timeStamp);
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                //ข้อยกเว้นเฉพาะ ให้มี .@ ในที่อยู่อีเมล์ได้
                string email2 = email.Replace(".@", "@");
                var addr = new System.Net.Mail.MailAddress(email2);
                return addr.Address == email2;
            }
            catch
            {
                return false;
            }
        }

        public static string GetCurrentMethod(Int32 index = 2)
        {
            var st = new StackTrace();
            MethodBase method = st.GetFrame(index).GetMethod();
            string methodName = method.Name;
            //string className = method.ReflectedType.Name;
            //string fullMethodName = string.Format("{0}.{1}", className, methodName);
            //return fullMethodName;
            return methodName;
        }

        public static CultureInfo GetCultureInfo(string cultureName = Constants.KnownCulture.EnglishUS)
        {
            var ciUI = new CultureInfo(cultureName);
            return ciUI;
        }

        public static String GetCopyright()
        {
            string currentYear = DateTime.Today.Year > Constants.CompanyStartYear
                ? String.Format("-{0}", DateTime.Today.Year)
                : string.Empty;
            return string.Format(Resource.Lbl_Copyright, Constants.CompanyStartYear, currentYear);
        }

        public static string RemoveExtraSpaces(this string line)
        {
            try
            {
                return Regex.Replace(line, @"\s+", " ");
            }
            catch (Exception)
            {
                return line;
            }
        }

        public static bool Authenticate(string username, string password)
        {
            var taskUsername = WebConfig.GetTaskUsername();
            var taskPassword = WebConfig.GetTaskPassword();
            var isValidTaskUser = taskUsername.Equals(username) && taskPassword.Equals(password);

            var webUsername = WebConfig.GetWebUsername();
            var webPassword = WebConfig.GetWebPassword();
            var isValidWebUser = webUsername.Equals(username) && webPassword.Equals(password);

            return (isValidTaskUser || isValidWebUser);
        }

        public static string StripHtmlTags(string html)
        {
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(html);
            htmldoc.OptionWriteEmptyNodes = true;
            htmldoc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style" || n.Name == "meta" || n.Name == "base").ToList().ForEach(n => n.Remove());
            //var allText = htmldoc.DocumentNode.SelectNodes("//div | //ul | //p | //table | //font");
            var allText = htmldoc.DocumentNode.SelectNodes("//*");

            if (allText != null)
            {
                foreach (var node in allText)
                {
                    //node.Attributes.Remove("class");
                    //node.Attributes.Remove("style");
                    node.Attributes.Remove("face");
                    node.Attributes.Remove("size");

                    if (node.Name == "img")
                    {
                        node.Attributes.Add("class", "img-responsive");
                        node.Attributes.Add("onerror", "this.style.display='none';");
                    }
                }
            }

            string cleanHtml = !string.IsNullOrWhiteSpace(htmldoc.DocumentNode.InnerHtml) ? htmldoc.DocumentNode.InnerHtml : htmldoc.DocumentNode.InnerText;

            if (htmldoc.ParseErrors.Count() > 0)
            {
                Logger.DebugFormat("I:--START--:--Strip Html Tags--:Invalid HTML/{0}", html);
            }

            return cleanHtml;
        }

        public static string RemoveHtmlTags(string html)
        {
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(html);
            htmldoc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style" || n.Name == "meta" || n.Name == "base").ToList().ForEach(n => n.Remove());
            string s = Regex.Replace(htmldoc.DocumentNode.InnerHtml, @"<[^>]+>|&nbsp;", string.Empty).NullSafeTrim();
            return s;
        }

        public static string RemoveAllHtmlTags(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            //string result = html.ReplaceBreaks().RemoveCommentHtmlTags();
            string result = html.Replace("\r\n", "").Replace("\n", "").ReplaceBreaks().RemoveCommentHtmlTags();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            return HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
        }

        public static string ReplaceBreaks(this string value)
        {
            return Regex.Replace(value, @"(<br */>)|(\[br */\])", "\n");
        }

        public static string RemoveCommentHtmlTags(this string value)
        {
            return Regex.Replace(value, "<!--.*?-->", "", RegexOptions.Singleline);
        }

        public static string GetMessageKey(string errorSystem, string errorService, string errorCode)
        {
            string[] names = new string[3] { errorSystem.NullSafeTrim(), errorService.NullSafeTrim(), errorCode.NullSafeTrim() };

            if (names.Any(x => !string.IsNullOrEmpty(x)))
            {
                return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + "_" + j);
            }

            return string.Empty;
        }

        public static string GenerateFileName(string documentFolder, string fileExtension, int attachmentSeq, string attachmentPrefix)
        {
            string folderYear = DateTime.Now.FormatDateTime("yyyy");
            string folderMonth = DateTime.Now.FormatDateTime("MM");

            // Check Exists Folder           
            if (Directory.Exists(string.Format("{0}\\{1}", documentFolder, folderYear)) == false)
            {
                Directory.CreateDirectory(string.Format("{0}\\{1}", documentFolder, folderYear));
            }
            if (Directory.Exists(string.Format("{0}\\{1}\\{2}", documentFolder, folderYear, folderMonth)) == false)
            {
                Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", documentFolder, folderYear, folderMonth));
            }

            string year = DateTime.Now.FormatDateTime("yyMMdd");
            string randomNo = (new Random()).Next(0, 99).ToString("00");
            string seqNo = attachmentSeq.ToString("0000000");
            string fileName = string.Format("{0}{1}{2}", year, seqNo, randomNo);
            fileName += GetDigit(fileName);

            return string.Format("{0}\\{1}\\{2}{3}{4}", folderYear, folderMonth, attachmentPrefix, fileName, fileExtension);
        }

        public static string GenerateSrNo(int attachmentSeq)
        {
            string year = DateTime.Now.FormatDateTime("yy", Constants.KnownCulture.Thai);
            string randomNo = (new Random()).Next(0, 99).ToString("00");
            string seqNo = attachmentSeq.ToString("0000000");
            string fileName = string.Format("{0}{1}{2}", year, seqNo, randomNo);
            fileName += GetDigit(fileName);

            return fileName;
        }

        private static string GetDigit(string value)
        {
            int digit = 0;
            int total = 0;
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                total += int.Parse(value.ToArray().GetValue(i).ToString()) * (length - i);
            }

            digit = length - (total % length);

            return (digit % (length - 1)).ConvertToString();
        }

        public static bool ValidateCardNo(string cardNo)
        {
            try
            {
                if (cardNo.Length != 13) return false;

                int sum = 0;
                for (int i = 0; i < 12; i++)
                {
                    sum += int.Parse(cardNo.ToArray().GetValue(i).ToString()) * (13 - i);
                }
                if ((11 - sum % 11) % 10 != (int.Parse(cardNo.ToArray().GetValue(12).ToString())))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static Dictionary<string, string> GetParams(string uri)
        {
            var parsedString = HttpUtility.UrlDecode(uri);
            var matches = Regex.Matches(parsedString, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            var keyValues = new Dictionary<string, string>(matches.Count);

            foreach (Match m in matches)
            {
                keyValues.Add(Uri.UnescapeDataString(m.Groups[2].Value).ToLower(),
                    Uri.UnescapeDataString(m.Groups[3].Value));
            }

            return keyValues;
        }

        public static dynamic[] DoValidation(object toValidate)
        {
            bool validateAllProperties = false;
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                toValidate,
                new ValidationContext(toValidate, null, null),
                results,
                validateAllProperties);

            return new dynamic[] { isValid, results };
        }

        public static bool IsValidEmailDomain(string s)
        {
            Match match = Regex.Match(s, @"^[a-zA-Z0-9._%+-]+@kiatnakin.co.th$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                Logger.InfoFormat("O:--Check Valid Email Domain--:Invalid Email/False:/EmailAddress/{0}", s);
                return false;
            }

            return true;
        }

        public static string GetSenderAddress(string s)
        {
            int pos = s.IndexOf('@');
            string senderName = pos != -1 ? s.Substring(0, pos) : string.Empty;

            if (!string.IsNullOrWhiteSpace(senderName))
            {
                var match = Regex.Match(senderName, @"[^\/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match.Success)
                {
                    return match.Value.NullSafeTrim();
                }
            }

            return senderName;
        }

        public static string GetBase64Image(string path)
        {
            Image image = null;

            try
            {
                byte[] imageBytes;
                image = Image.FromFile(path);

                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    imageBytes = m.ToArray();
                }

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (image != null) { image.Dispose(); }
            }

            return null;
        }

        public static string GetBase64Image(byte[] imageBytes)
        {
            try
            {
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return null;
        }

        public static string StringLimit(string value, int length)
        {
            try
            {
                string result = value;
                if (!string.IsNullOrEmpty(value) && value.Count() > length)
                {
                    result = value.Substring(0, length);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return "";
        }

        public static string GetFileFormat(string fiPrefix, DateTime dt, string pattern)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}.txt", fiPrefix, dt.FormatDateTime(pattern));
        }

        #region "Get latest updated date of the DLL"

        public static string GetDisplayDllLastUpdatedDate()
        {
            string softwareVersion = WebConfig.GetSoftwareVersion();
            return softwareVersion;
        }

        private static DateTime GetDllLastUpdatedDate()
        {
            var latestDate = new DateTime();
            var dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/bin"));
            FileInfo[] arrFileInfo = dirInfo.GetFiles("*.dll");
            foreach (FileInfo fileInfo in arrFileInfo)
            {
                if (fileInfo.LastWriteTime > latestDate)
                {
                    latestDate = fileInfo.LastWriteTime;
                }
            }

            return latestDate;
        }

        #endregion

        public static string GetDisplayMinimunRequirement()
        {
            return WebConfig.GetDisplayMinimunRequirement();
        }

        #region "My Extensions"

        public static T? ToNullable<T>(this string s) where T : struct
        {
            var result = new T?();
            try
            {
                if (!string.IsNullOrWhiteSpace(s) && s.Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)conv.ConvertFromInvariantString(s);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return result;
        }

        public static string FormatDecimal<T>(this T obj)
        {
            string result = string.Empty;

            try
            {
                if (obj != null)
                {
                    result = String.Format("{0:#,##0.00}", obj);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FormatException:\n", ex);
            }

            return result;
        }

        public static string FormatNumber<T>(this T obj)
        {
            string result = string.Empty;

            try
            {
                if (obj != null)
                {
                    result = String.Format("{0:#,##0}", obj);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FormatException:\n", ex);
            }

            return result;
        }

        public static T CopyObject<T>(this object objSource)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, objSource);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T? ParseDecimal<T>(this string s) where T : struct
        {
            string result = string.Empty;

            try
            {
                if (s.IndexOf(",") != -1)
                {
                    result = s.Trim().Replace(",", string.Empty);
                }
                else
                {
                    result = s.Trim();
                }

                return result.ToNullable<T>();
            }
            catch (Exception ex)
            {
                Logger.Error("FormatException:\n", ex);
                return new T?();
            }
        }

        public static string MaskCardNo(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            string cardNo = str.Length < 13 ? str.PadLeft(13, 'X') : str;
            return string.Concat("".PadLeft(8, 'X'), cardNo.Substring(cardNo.Length - 5));
        }

        public static IDictionary<string, T> ToDictionary<T>(this T values)
        {
            var dict = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

            if (values != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
                {
                    var obj = (T)propertyDescriptor.GetValue(values);
                    dict.Add(propertyDescriptor.Name, obj);
                }
            }

            return dict;
        }

        public static Boolean ToBoolean(this string str)
        {
            try
            {
                return Convert.ToBoolean(str);
            }
            catch (FormatException)
            {
                Logger.ErrorFormat("Cannot Converted String ({0}) to Boolean", str);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            try
            {
                return Convert.ToBoolean(Convert.ToInt32(str));
            }
            catch (FormatException)
            {
                Logger.ErrorFormat("Cannot Converted String ({0}) to Boolean", str);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public static string TextToHtml(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                return HttpUtility.HtmlEncode(s).Replace("\n", "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            return string.Empty;
        }

        public static string ExtractSRStatus(this string s)
        {
            var match = Regex.Match(s, @"(?<=\().+?(?=\))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Value.NullSafeTrim();
            }

            return string.Empty;
        }

        public static string ExtractSRNo(this string s)
        {
            var match = Regex.Match(s, @";;|;\s*[0-9]{1,}\s*;", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Value.Replace(";", string.Empty).NullSafeTrim();
            }

            return string.Empty;
        }

        public static string ExtractDataField(this string s, string searchText)
        {
            Logger.DebugFormat("I:--START--:--Extract Data Field--:StringValue/{0}:SearchText/{1}", s, searchText);

            if (!string.IsNullOrWhiteSpace(s))
            {
                var match = Regex.Match(s, string.Format(@"{0}\s*:*\s.*?:(.*)", searchText), RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match.Success)
                {
                    string result = match.Groups[1].Value.NullSafeTrim();
                    Logger.DebugFormat("O:--SUCCESS--:--Extract Data Field--:StringValue/{0}:SearchText/{1}/Result/{2}", s, searchText, result);
                    return result;
                }
            }
            else
            {
                Logger.Debug("O:--FAILED--:--Extract Data Field--:Error Message/StringValue is null");
            }

            return string.Empty;
        }

        public static void RemoveByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue someValue)
        {
            List<TKey> itemsToRemove = new List<TKey>();

            foreach (var pair in dictionary)
            {
                if (pair.Value.Equals(someValue))
                    itemsToRemove.Add(pair.Key);
            }

            foreach (TKey item in itemsToRemove)
            {
                dictionary.Remove(item);
            }
        }

        public static string ExtractString(this string s, ref int searchType)
        {
            string[] patterns = new string[3] { @"^(?!\*).*\*+$", @"^\*.*(?!\*).$", @"^\*+.*\*+$" };
            for (int i = 0; i < patterns.Length; i++)
            {
                Match match = Regex.Match(s, patterns[i], RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match.Success)
                {
                    searchType = i + 1;
                    return Regex.Replace(s, @"^\*+|\*+$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline).NullSafeTrim();
                }
            }

            return s;
        }

        public static string ExtractString(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                int refSearchType = 0;
                return s.ExtractString(ref refSearchType);
            }

            return string.Empty;
        }

        public static string EscapeSingleQuotes(this string s)
        {
            try
            {
                return s.Replace("'", "''");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return s;
        }

        public static bool IsQueryStringExists(this string url, string key)
        {
            var match = Regex.Match(url, @"(returnUrl=[^&]*|returnUrl=[^&]*&)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return match.Success;
        }

        public static string ToSLMTimeFormat(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                string time = s.Replace(":", string.Empty);
                return time.PadRight(6, '0');
            }

            return null;
        }

        public static string JSEncode(this string value)
        {
            return HttpUtility.UrlEncode(value).Replace("+", "%20");
        }

        public static string ToErrorMessage(this AggregateException exception)
        {
            StringBuilder sb = new StringBuilder("");

            if (exception.InnerExceptions != null && exception.InnerExceptions.Count > 0)
            {
                foreach (Exception ex in exception.InnerExceptions)
                {
                    sb.AppendFormat(Constants.StackTraceError.InnerException, ex.Source, ex.Message, ex.StackTrace);
                }
            }

            if (exception.InnerException != null)
            {
                string innerException = sb.ToString();
                sb.Clear();
                sb.AppendFormat(Constants.StackTraceError.Exception, innerException);
            }

            return sb.ToString();
        }

        public static int? ToCustomerId(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                try
                {
                    var encrypted = HttpUtility.HtmlDecode(str);
                    var decrypted = StringCipher.Decrypt(encrypted, Constants.PassPhrase);
                    return decrypted.ToNullable<int>();
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                }
            }

            return null;
        }

        public static string ExtractContentID(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                var match = Regex.Match(s, @"\<(.*?)\>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match.Success)
                {
                    return match.Groups[1].Value.NullSafeTrim();
                }
            }

            return string.Empty;
        }

        public static bool IsImage(this string contentType)
        {
            return (!string.IsNullOrWhiteSpace(contentType) && contentType.Contains("image"));
        }

        public static bool InList<T>(this T val, params T[] list)
        {
            return list.Contains(val);
        }

        public static string ToDisplay(this bool? val, string nullDisplay = "")
        {
            return (!val.HasValue ? nullDisplay : (val.Value ? "Yes" : "No"));
        }

        public static string ToDisplay(this DateTime? val, string nullDisplay = "")
        {
            return (!val.HasValue ? nullDisplay : (val.Value.FormatDateTime(Constants.DateTimeFormat.ReportDateTime)));
        }

        public static string IsActiveToDisplay(this bool val) { return (val ? "Active" : "Inactive"); }

        public static string ToDayHourMinute(this int? minute)
        {
            if (minute.HasValue == false) { return string.Empty; }
            var val = minute;
            var dd = val / 1440;
            var hh = (val / 60) % 24;
            var mm = val % 60;
            var ret = (dd > 0 ? dd + "D " : "") + (hh > 0 ? hh + "H " : "") + (mm > 0 ? mm + "MM" : "");
            return string.IsNullOrWhiteSpace(ret) ? "0MM" : ret;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string StripNewLine(string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", String.Empty);
        }

        #endregion
    }
}