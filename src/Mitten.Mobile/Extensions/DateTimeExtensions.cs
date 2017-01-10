using System;
using System.Globalization;

namespace Mitten.Mobile.Extensions
{
    /// <summary>
    /// Contains DateTime extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        private static class Constants
        {
            public const string FullMonthFormat = "MMMM";
            public const string AbbreviatedMonthFormat = "MMM";

            public const string FullDayFormat = "dddd";
            public const string AbbreviatedDayFormat = "ddd";

            public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
            public static readonly DateTime UnixEpochMax = new DateTime(2038, 1, 19);
        }

        /// <summary>
        /// Determines whether or not the specified Date is the same as today's date. 
        /// </summary>
        /// <param name="value">A value to check.</param>
        /// <returns>True if the specified date is the same as today.</returns>
        public static bool IsToday(this DateTime value)
        {
            return value.ToLocalTime().Date == DateTime.Today;
        }

        /// <summary>
        /// Determines whether or not the specified Date represents tomorrow's date. 
        /// </summary>
        /// <param name="value">A value to check.</param>
        /// <returns>True if the specified date is for tomorrow.</returns>
        public static bool IsTomorrow(this DateTime value)
        {
            return value.ToLocalTime().Date - DateTime.Today == TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Converts the DateTime into a string and ensures the date and time is presented in local time.
        /// </summary>
        /// <param name="dateTime">The date and time to convert.</param>
        /// <returns>A string representing the date and time.</returns>
        public static string ToLocalAbbreviatedFullString(this DateTime dateTime)
        {
            const string secondsPart = ":ss";

            string format = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;

            format = format.Replace(Constants.FullDayFormat, Constants.AbbreviatedDayFormat);
            format = format.Replace(Constants.FullMonthFormat, Constants.AbbreviatedMonthFormat);
            format = format.Replace(secondsPart, string.Empty);

            return DateTimeExtensions.ToLocalString(dateTime, format);
        }

        /// <summary>
        /// Converts the DateTime to a Unix timestamp.
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        /// <returns>The total number of seconds since the Unix Epoch.</returns>
        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            if (dateTime < Constants.UnixEpoch || dateTime > Constants.UnixEpochMax)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime));
            }

            TimeSpan timeSpan = dateTime - Constants.UnixEpoch;
            return (int)timeSpan.TotalSeconds;
        }

        private static string ToLocalString(DateTime dateTime, string format)
        {
            return dateTime.ToLocalTime().ToString(format);
        }
    }
}
