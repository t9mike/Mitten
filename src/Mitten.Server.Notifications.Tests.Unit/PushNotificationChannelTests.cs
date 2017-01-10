using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mitten.Server.Notifications.Events;
using Mitten.Server.Notifications.Push;
using NSubstitute;
using NUnit.Framework;

namespace Mitten.Server.Notifications.Tests.Unit
{
    [TestFixture]
    public class PushNotificationChannelTests
    {
        private static class Constants
        {
            public const string PushNotificationEndPoint = "Test";
        }

        [Test]
        public void SendPushNotificationWithNoAppVersionSpecifiedTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification();
            NotificationAccount<Guid> account = test.AddNotificationAccount();

            channel.SendAsync(account, notification).Wait();

            this.AssertSentSuccessful(test, account, notification);
        }

        [Test]
        public void SendPushNotificationWithAppVersionSpecifiedTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification(new Version(1, 0, 0), new Version(1, 0, 2));
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), mobileAppVersion: "1.0.0");

            channel.SendAsync(account, notification).Wait();

            this.AssertSentSuccessful(test, account, notification);
        }

        [Test]
        public void SendPushNotificationWithOnlyMinimumAppVersionSpecifiedTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification(new Version(1, 0, 0));
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), mobileAppVersion: "1.0.3");

            channel.SendAsync(account, notification).Wait();

            this.AssertSentSuccessful(test, account, notification);
        }

        [Test]
        public void SendPushNotificationForAnOutdatedVersionOfTheApplicationTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification(new Version(1, 0, 0), new Version(1, 0, 2));
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), mobileAppVersion: "1.0.3");

            channel.SendAsync(account, notification).Wait();

            this.AssertSentFailed(test, account, notification, NotificationErrorCode.VersionMismatch);
        }

        [Test]
        public void SendVersionedPushNotificationToDeviceWithoutVersionTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification(new Version(1, 0, 0));
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), mobileAppVersion: null);

            channel.SendAsync(account, notification).Wait();

            this.AssertSentFailed(test, account, notification, NotificationErrorCode.VersionMismatch);
        }

        [Test]
        public void SendVersionedPushNotificationToDeviceWithoutWithInvalidVersionTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification(new Version(1, 0, 0));
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), mobileAppVersion: "Invalid");

            channel.SendAsync(account, notification).Wait();

            this.AssertSentFailed(test, account, notification, NotificationErrorCode.VersionMismatch);
        }

        [Test]
        public void SendPushNotificationForAccountWithoutMobileDeviceTest()
        {
            NotificationTest test = new NotificationTest();
            PushNotificationChannel channel = new PushNotificationChannel(this.CreateMobileServiceClient(), test.EventPublisher);

            PushNotification notification = this.CreateNotification();
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), "joe@g.com", "Joe", null, (MobileDevice)null);

            channel.SendAsync(account, notification).Wait();

            this.AssertSentFailed(test, account, notification, NotificationErrorCode.MobileDeviceNotRegistered);
        }

        [Test]
        public void SendInvalidEndPointPushNotificationTest()
        {
            NotificationTest test = new NotificationTest();
            IMobilePushNotificationServiceClient mobileServiceClient = this.CreateMobileServiceClient();
            PushNotificationChannel channel = new PushNotificationChannel(mobileServiceClient, test.EventPublisher);

            PushNotification notification = this.CreateNotification();
            NotificationAccount<Guid> account = test.AddNotificationAccount();

            this.PushNotificationShouldFailAtEndPoint(mobileServiceClient);

            channel.SendAsync(account, notification).Wait();

            this.AssertSentFailed(test, account, notification, NotificationErrorCode.RouteFailure);
        }

        private void AssertSentSuccessful(NotificationTest test, NotificationAccount<Guid> account, PushNotification expectedNotification)
        {
            PushNotification sentNotification = (PushNotification)test.GetSentNotification();

            this.AssertNotification(sentNotification, expectedNotification);
            this.AssertNotificationSentEventRaised(test, account, expectedNotification);
        }

        private void AssertSentFailed(NotificationTest test, NotificationAccount<Guid> account, PushNotification expectedNotification, NotificationErrorCode expectedErrorCode)
        {
            NotificationSendFailure notificationSendFailure = test.RaisedEvents.OfType<NotificationSendFailure>().SingleOrDefault();

            notificationSendFailure.Should().NotBeNull();
            notificationSendFailure.AccountId.ShouldBeEquivalentTo(account.AccountId.ToString());
            notificationSendFailure.Description.Should().NotBeNull();
            notificationSendFailure.ErrorCode.ShouldBeEquivalentTo(expectedErrorCode);

            if (expectedErrorCode == NotificationErrorCode.RouteFailure)
            {
                notificationSendFailure.EndpointName.ShouldBeEquivalentTo(Constants.PushNotificationEndPoint);
            }
            else
            {
                notificationSendFailure.EndpointName.Should().BeNull();
            }

            if (account.MobileDevices != null && account.MobileDevices.Any())
            {
                notificationSendFailure.Destination.ShouldBeEquivalentTo(account.MobileDevices.Single().DeviceId);
            }
            else
            {
                notificationSendFailure.Destination.Should().BeNull();
            }

            this.AssertNotification((PushNotification)notificationSendFailure.Notification, expectedNotification);
        }

        private void AssertNotificationSentEventRaised(NotificationTest test, NotificationAccount<Guid> account, PushNotification expectedNotification)
        {
            NotificationSent notificationSent = test.RaisedEvents.OfType<NotificationSent>().SingleOrDefault();

            notificationSent.Should().NotBeNull();
            notificationSent.AccountId.ShouldBeEquivalentTo(account.AccountId.ToString());
            notificationSent.Notification.NotificationType.ShouldBeEquivalentTo(NotificationType.Push);

            string expectedDeviceId = null;
            if (account.MobileDevices != null &&
                account.MobileDevices.Any())
            {
                MobileDevice device = account.MobileDevices.Single();
                expectedDeviceId = device.DeviceId;
            }

            notificationSent.Destination.ShouldBeEquivalentTo(expectedDeviceId);
            notificationSent.EndpointName.ShouldBeEquivalentTo(Constants.PushNotificationEndPoint);
            
            this.AssertNotification((PushNotification)notificationSent.Notification, expectedNotification);
        }

        private void AssertNotification(PushNotification actual, PushNotification expected)
        {
            actual.AlertText.ShouldBeEquivalentTo(expected.AlertText);
            actual.Content.ShouldAllBeEquivalentTo(expected.Content);
            actual.Id.ShouldBeEquivalentTo(expected.Id);
            actual.MaximumAppVersion.ShouldBeEquivalentTo(expected.MaximumAppVersion);
            actual.MinimumAppVersion.ShouldBeEquivalentTo(expected.MinimumAppVersion);
            actual.Name.ShouldBeEquivalentTo(expected.Name);
            actual.NotificationType.ShouldBeEquivalentTo(expected.NotificationType);
            actual.Attributes.ShouldAllBeEquivalentTo(expected.Attributes);
        }

        private IMobilePushNotificationServiceClient CreateMobileServiceClient()
        {
            IMobilePushNotificationServiceClient client = Substitute.For<IMobilePushNotificationServiceClient>();

            client
                .SendNotification(Arg.Any<MobileDevice>(), Arg.Any<PushNotification>())
                .Returns(PushNotificationResult.Success(Constants.PushNotificationEndPoint));

            return client;
        }

        private void PushNotificationShouldFailAtEndPoint(IMobilePushNotificationServiceClient client)
        {
            client
                .SendNotification(Arg.Any<MobileDevice>(), Arg.Any<PushNotification>())
                .Returns(PushNotificationResult.Failed(Constants.PushNotificationEndPoint, "Invalid notification."));
        }

        private PushNotification CreateNotification(Version minimumAppVersion = null, Version maximumAppVersion = null)
        {
            return
                new PushNotification(
                    "TestNotification",
                    "This is a test.",
                    new[] { new KeyValuePair<string, string>("key", "some value") },
                    minimumAppVersion,
                    maximumAppVersion);
        }
    }
}