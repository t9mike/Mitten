using System;
using System.Globalization;
using FluentAssertions;
using Mitten.Server.Extensions;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit
{
    [TestFixture]
    public class DateTimeConverterTests
    {
        [Test]
        public void ConvertFromUnixSecondsTest()
        {
            const int timeInSeconds = 1461761684;
            
            DateTime date = DateTimeConverter.FromUnixTimestamp(timeInSeconds);

            DateTime expectedDate = DateTime.Parse("2016-04-27T12:54:44+00:00", null, DateTimeStyles.AdjustToUniversal);
            date.ShouldBeEquivalentTo(expectedDate);
        }

        [Test]
        public void ConvertToUnixSecondsTest()
        {
            DateTime date = DateTime.Parse("2016-04-27T12:54:44+00:00", null, DateTimeStyles.AdjustToUniversal);
            
            int timeInSeconds = date.ToUnixTimestamp();
            
            timeInSeconds.ShouldBeEquivalentTo(1461761684);
        }
    }
}
