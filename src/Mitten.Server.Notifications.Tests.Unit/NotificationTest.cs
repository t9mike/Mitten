using System;
using System.Collections.Generic;
using System.Linq;
using Mitten.Server.Events;
using Mitten.Server.Notifications.Events;
using NSubstitute;
using NSubstitute.Core;

namespace Mitten.Server.Notifications.Tests.Unit
{
    /// <summary>
    /// Contains helper methods for setting up and running tests.
    /// </summary>
    public class NotificationTest
    {
        private static class Constants
        {
            public const string PushNotificationToken = "token";
            public const string MobileDeviceId = "device id";
        }

        private readonly Lazy<INotificationAccountRepository<Guid>> notificationAccountRepository;
        private readonly Lazy<INotificationRepository<Guid>> notificationRepository;

        private readonly List<IEvent> raisedEvents;

        /// <summary>
        /// Initializes a new instance of the NotificationTest class.
        /// </summary>
        public NotificationTest()
        {
            this.notificationAccountRepository = this.CreateLazySubstitute<INotificationAccountRepository<Guid>>();
            this.notificationRepository = this.CreateLazySubstitute<INotificationRepository<Guid>>();

            this.raisedEvents = new List<IEvent>();
            this.EventPublisher = Substitute.For<IEventPublisher>();
            this.EventPublisher
                .When(publisher => publisher.Publish(Arg.Any<IEvent>()))
                .Do(callInfo => this.raisedEvents.Add(callInfo.Arg<IEvent>()));
        }

        /// <summary>
        /// Gets a notification account repository.
        /// </summary>
        public INotificationAccountRepository<Guid> NotificationAccountRepository => this.notificationAccountRepository.Value;

        /// <summary>
        /// Gets a notification repository.
        /// </summary>
        public INotificationRepository<Guid> NotificationRepository => this.notificationRepository.Value;

        /// <summary>
        /// Gets an event publisher for the test.
        /// </summary>
        public IEventPublisher EventPublisher { get; }

        /// <summary>
        /// Gets a list of events that were raised during the test.
        /// </summary>
        public IEnumerable<IEvent> RaisedEvents => this.raisedEvents;

        /// <summary>
        /// Gets a notification if one was sent.
        /// </summary>
        /// <returns>The sent notification.</returns>
        public Notification GetSentNotification()
        {
            NotificationSent notificationSent = this.raisedEvents.OfType<NotificationSent>().SingleOrDefault();

            if (notificationSent == null)
            {
                throw new InvalidOperationException("A notification was not sent.");
            }

            return notificationSent.Notification;
        }

        /// <summary>
        /// Gets a notification if one was scheduled.
        /// </summary>
        /// <returns>The scheduled notification.</returns>
        public ScheduledNotification<Guid> GetScheduledNotification()
        {
            ICall saveScheduledNotification =
                this.NotificationRepository
                    .ReceivedCalls()
                    .SingleOrDefault(call => call.GetMethodInfo().Name == nameof(INotificationRepository<Guid>.SaveScheduledNotification));

            if (saveScheduledNotification == null)
            {
                throw new InvalidOperationException("A notification was not scheduled.");
            }

            return saveScheduledNotification.GetArguments().Cast<ScheduledNotification<Guid>>().Single();
        }

        /// <summary>
        /// Adds a notification account to the test.
        /// </summary>
        /// <returns>The account that was added to the repository.</returns>
        public NotificationAccount<Guid> AddNotificationAccount()
        {
            return this.AddNotificationAccount(Guid.NewGuid());
        }

        /// <summary>
        /// Adds a notification account to the test.
        /// </summary>
        /// <param name="accountId">An account identifier.</param>
        /// <param name="email">An email for the account.</param>
        /// <param name="name">A name for the account.</param>
        /// <param name="timeZone">The time zone for the account.</param>
        /// <param name="mobileAppVersion">The version for the app running on a mobile device.</param>
        /// <returns>The account that was added to the repository.</returns>
        public NotificationAccount<Guid> AddNotificationAccount(
            Guid accountId,
            string email = "joe@g.com",
            string name = "Joe",
            string timeZone = "US/Eastern",
            string mobileAppVersion = "1.0.0")
        {
            return
                this.AddNotificationAccount(
                    accountId,
                    email,
                    name,
                    timeZone,
                    new MobileDevice(Constants.MobileDeviceId, MobileDevicePlatformType.iOS, mobileAppVersion, true, Constants.PushNotificationToken));
        }

        /// <summary>
        /// Adds a notification account to the test.
        /// </summary>
        /// <param name="accountId">An account identifier.</param>
        /// <param name="email">An email for the account.</param>
        /// <param name="name">A name for the account.</param>
        /// <param name="timeZone">The time zone for the account.</param>
        /// <param name="mobileDevice">The mobile device to associate with the account.</param>
        /// <returns>The account that was added to the repository.</returns>
        public NotificationAccount<Guid> AddNotificationAccount(Guid accountId, string email, string name, string timeZone, MobileDevice mobileDevice)
        {
            NotificationAccount<Guid> account =
                mobileDevice != null
                ? new NotificationAccount<Guid>(accountId, email, name, timeZone, new[] { mobileDevice })
                : new NotificationAccount<Guid>(accountId, email, name, timeZone, null);

            this.NotificationAccountRepository.GetAccount(accountId).Returns(account);

            return account;
        }

        private Lazy<T> CreateLazySubstitute<T>()
            where T : class
        {
            return new Lazy<T>(() => Substitute.For<T>());
        }
    }
}