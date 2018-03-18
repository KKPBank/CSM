using System;
using System.Globalization;

///<summary>
/// Class Name : DateTimeHelpers
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date            Author          Description
/// ---             ---             ------
///</remarks>
namespace CSM.Common.Utilities
{
    public static class DateTimeHelpers
    {
        private static readonly DateTime? nullDateTime = new DateTime?();

        #region "My Extensions"

        public static String FormatDateTime(this DateTime? date, string pattern)
        {
            string result = null;

            if (date.HasValue)
            {
                result = date.Value.ToString(pattern, CultureInfo.InvariantCulture);
            }

            return result;
        }

        public static String FormatDateTime(this DateTime date, string pattern)
        {
            return date.ToString(pattern, CultureInfo.InvariantCulture);
        }

        public static string FormatDateTime(this DateTime date, string pattern, string culture)
        {
            return date.ToString(pattern, ApplicationHelpers.GetCultureInfo(culture));
        }

        public static DateTime? ParseDateTime(this string strDate, string pattern)
        {
            DateTime date;

            bool noError = DateTime.TryParseExact(strDate,
                pattern,
                CultureInfo.InvariantCulture,
                DateTimeStyles.NoCurrentDateDefault,
                out date);

            return noError ? date : nullDateTime;
        }

        public static String FormatTimestamp(this DateTime value)
        {
            return FormatDateTime(value, "yyyyMMddHHmmssffffff");
        }

        #endregion
    }
}