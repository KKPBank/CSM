using System;
using System.Globalization;

namespace CSM.Common.Utilities
{
    public class DateUtil
    {
        public static string DefaultDateFormat
        {
            get { return "dd/MM/yyyy HH:mm:ss"; }
        }
        public static string DefaultDateOnlyFormat
        {
            get { return "dd/MM/yyyy"; }
        }
        
        public static CultureInfo DefaultCultureInfo
        {
            get { return new CultureInfo(DefaultCultureName); }
        }
        public static string DefaultCultureName
        {
            get { return "en-US"; }
        }
        public static string ToStringAsDateTime(DateTime? value)
        {

            if (!value.HasValue)
                return string.Empty;

            return value.Value.ToString(DefaultDateFormat, DefaultCultureInfo);
        }
        public static string ToStringAsDate(DateTime? value)
        {
            if (!value.HasValue)
                return string.Empty;

            return value.Value.ToString(DefaultDateOnlyFormat, DefaultCultureInfo);
        }
    }
}
