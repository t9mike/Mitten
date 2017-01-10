using System;
using FluentAssertions;
using Mitten.Server.Extensions;
using NUnit.Framework;

namespace Mitten.Server.Tests.Unit
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void TryParseAsIntTest()
        {
            const string value = "10";

            int? result = value.TryParseAsInt();

            result.HasValue.Should().BeTrue();
            result.Value.ShouldBeEquivalentTo(10);
        }

        [Test]
        public void TryParseNullAsIntTest()
        {
            const string value = null;

            int? result = value.TryParseAsInt();

            result.HasValue.Should().BeFalse();
        }

        [Test]
        public void TryParseInvalidValueAsIntTest()
        {
            const string value = "foo";

            int? result = value.TryParseAsInt();

            result.HasValue.Should().BeFalse();
        }

        [Test]
        public void TryParseAsGuidTest()
        {
            const string value = "F947FCBC-1D21-4C8B-9437-E5A8E387A2E0";

            Guid? result = value.TryParseAsGuid();

            result.HasValue.Should().BeTrue();
            result.Value.ShouldBeEquivalentTo(Guid.Parse(value));
        }

        [Test]
        public void TryParseNullAsGuidTest()
        {
            const string value = null;

            Guid? result = value.TryParseAsGuid();

            result.HasValue.Should().BeFalse();
        }

        [Test]
        public void TryParseInvalidValueAsGuidTest()
        {
            const string value = "foo";

            Guid? result = value.TryParseAsGuid();

            result.HasValue.Should().BeFalse();
        }
    }
}
