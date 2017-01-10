using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mitten.Server.Extensions;
using Mitten.Server.Notifications.Events;
using Mitten.Server.Notifications.Push;
using NSubstitute;
using NUnit.Framework;

namespace Mitten.Server.Notifications.Tests.Unit
{
    [TestFixture]
    public class NotificationSchedulerTests
    {
        [Test]
        public void SchedulePushNotificationTest()
        {
            NotificationTest test = new NotificationTest();
            NotificationScheduler<Guid> scheduler = this.CreateScheduler(test);

            PushNotification notification = this.CreateNotification();
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), timeZone: "US/Pacific");

            DateTime deliveryDate = DateTime.UtcNow.AddDays(1);
            scheduler.Schedule(account.AccountId, notification, deliveryDate);

            DateTimeOffset expectedDeliveryDate = deliveryDate.ToDateTimeOffset("US/Pacific");
            this.AssertNotificationScheduledSuccessful(test, account, notification, expectedDeliveryDate);
        }

        [Test]
        public void SchedulePushNotificationWithoutTimeZoneTest()
        {
            NotificationTest test = new NotificationTest();
            NotificationScheduler<Guid> scheduler = this.CreateScheduler(test);

            PushNotification notification = this.CreateNotification();
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), timeZone: null);

            DateTime deliveryDate = DateTime.UtcNow.AddDays(1);
            scheduler.Schedule(account.AccountId, notification, deliveryDate);

            DateTimeOffset expectedDeliveryDate = deliveryDate.ToDateTimeOffset("US/Eastern");
            this.AssertNotificationScheduledSuccessful(test, account, notification, expectedDeliveryDate);
        }

        [Test]
        public void SendOverDueNotificationTest()
        {
            NotificationTest test = new NotificationTest();

            INotificationChannel channel;
            NotificationScheduler<Guid> scheduler = this.CreateScheduler(test, out channel);

            DateTime deliveryDate = DateTime.UtcNow;
            NotificationAccount<Guid> account = test.AddNotificationAccount(Guid.NewGuid(), "US/Pacific");
            PushNotification notificationToSend = this.CreateNotification();

            test.NotificationRepository.GetScheduledNotifications(Arg.Any<DateTime>()).Returns(new[] { new ScheduledNotification<Guid>(account.AccountId, notificationToSend, DateTime.UtcNow) });

            scheduler.SendScheduledNotifications(DateTime.UtcNow.AddDays(1)).Wait();

            channel.Received().SendAsync(account, notificationToSend);
            test.NotificationRepository.Received().DeleteScheduledNotification(notificationToSend.Id);
        }

        private void AssertNotificationScheduledSuccessful(NotificationTest test, NotificationAccount<Guid> account, Notification expectedNotification, DateTimeOffset expectedDeliveryDateTime)
        {
            ScheduledNotification<Guid> scheduledNotification = test.GetScheduledNotification();

            scheduledNotification.AccountId.ShouldBeEquivalentTo(account.AccountId);
            scheduledNotification.DeliveryDateTime.ShouldBeEquivalentTo(expectedDeliveryDateTime);

            this.AssertNotificationScheduledEventRaised(test, account, expectedNotification, expectedDeliveryDateTime);
        }

        private void AssertNotificationScheduledEventRaised(NotificationTest test, NotificationAccount<Guid> account, Notification expectedNotification, DateTimeOffset expectedDeliveryDateTime)
        {
            NotificationScheduled notificationScheduled = test.RaisedEvents.OfType<NotificationScheduled>().SingleOrDefault();

            notificationScheduled.Should().NotBeNull();
            notificationScheduled.AccountId.ShouldBeEquivalentTo(account.AccountId.ToString());
            notificationScheduled.DeliveryDateTime.ShouldBeEquivalentTo(expectedDeliveryDateTime);

            notificationScheduled.Notification.Should().BeSameAs(expectedNotification);
        }

        private NotificationScheduler<Guid> CreateScheduler(NotificationTest test)
        {
            INotificationChannel channel;
            return this.CreateScheduler(test, out channel);
        }

        private NotificationScheduler<Guid> CreateScheduler(NotificationTest test, out INotificationChannel channel)
        {
            channel = Substitute.For<INotificationChannel>();

            NotificationChannelFactory factory = new NotificationChannelFactory();
            factory.RegisterChannel<PushNotification>(channel);

            return
                new NotificationScheduler<Guid>(
                    new NotificationQueue(factory),
                    new NotificationAccountManager<Guid>(test.NotificationAccountRepository),
                    test.NotificationRepository,
                    test.EventPublisher);
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
