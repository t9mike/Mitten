using System;
using FluentAssertions;
using Mitten.Server.Extensions;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ConvertUTCToDateTimeOffsetTest()
        {
            DateTime date = new DateTime(2016, 3, 11, 12, 0, 0, DateTimeKind.Utc);
            DateTimeOffset dateOffset = date.ToDateTimeOffset("US/Pacific");
            
            dateOffset.DateTime.Hour.ShouldBeEquivalentTo(12);
            dateOffset.UtcDateTime.Hour.ShouldBeEquivalentTo(20);
        }
    }
}