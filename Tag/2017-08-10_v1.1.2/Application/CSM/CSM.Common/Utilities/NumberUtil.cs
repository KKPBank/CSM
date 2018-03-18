namespace CSM.Common.Utilities
{
    public class NumberUtil
    {
        public static string ToStringAsInt(int? i)
        {
            if (!i.HasValue)
                return string.Empty;

            return i.Value.ToString("0");
        }
    }
}