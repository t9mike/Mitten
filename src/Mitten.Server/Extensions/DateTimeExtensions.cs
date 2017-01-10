using System;
using NodaTime;

namespace Mitten.Server.Extensions
{
    /// <summary>
    /// Contains extension methods for date and time.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime into a DateTimeOffset for the specified IANA time zone name.
        /// </summary>
        /// <param name="dateTime">A date time to convert.</param>
        /// <param name="timeZone">A IANA time zone name.</param>
        /// <returns>A new date time offset.</returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, string timeZone)
        {
            Throw.IfArgumentNullOrWhitespace(timeZone, nameof(timeZone));

            DateTimeZone dateTimeZone = DateTimeZoneProviders.Tzdb[timeZone];
            LocalDateTime local = LocalDateTime.FromDateTime(dateTime);

            // The lenient method will automatically handle conflicting edge-cases such as daylight savings.
            // The alternative is to use strict which will instead throw an exception in such edge-cases.
            return local.InZoneLeniently(dateTimeZone).ToDateTimeOffset();
        }

        /// <summary>
        /// Converts a DateTime into a Unix timestamp which is the number of seconds since the Unix epoch.
        /// </summary>
        /// <param name="date">A date and time to convert.</param>
        /// <returns>The Unix timestamp.</returns>
        public static int ToUnixTimestamp(this DateTime date)
        {
            long ticksSinceUnixEpoch = Instant.FromDateTimeUtc(date).Ticks;
            return (int)(ticksSinceUnixEpoch / NodaConstants.TicksPerSecond);
        }
    }
}
