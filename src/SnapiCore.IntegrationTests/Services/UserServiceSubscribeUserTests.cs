using System;
using System.Threading.Tasks;
using SnapiCore.Services;
using Xunit;

namespace SnapiCore.IntegrationTests.Services
{
    public class UserServiceSubscribeUserTests
    {
        [Fact]
        public async Task Subscribe_NewSubscription_Subscribed()
        {
            var subscriberName = Guid.NewGuid().ToString();
            var subscriptionName = Guid.NewGuid().ToString();
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);

            var subscriber = await service.CreateUserAsync(subscriberName);
            var subscription = await service.CreateUserAsync(subscriptionName);

            var result = await service.SubscribeAsync(subscriberName, subscriptionName);

            Assert.Equal(SubscribeStatus.Subscribed, result.Status);
            Assert.NotNull(result.Value);
            Assert.Equal(subscriber.Value.Id, result.Value.FromId);
            Assert.Equal(subscription.Value.Id, result.Value.ToId);
        }


        [Fact]
        public async Task Subscribe_ExistsSubscription_AlreadySubscribed()
        {
            var subscriberName = Guid.NewGuid().ToString();
            var subscriptionName = Guid.NewGuid().ToString();
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);

            await service.CreateUserAsync(subscriberName);
            await service.CreateUserAsync(subscriptionName);

            await service.SubscribeAsync(subscriberName, subscriptionName);
            var result = await service.SubscribeAsync(subscriberName, subscriptionName);

            Assert.Equal(SubscribeStatus.AlreadySubscribed, result.Status);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Subscribe_NotExistsSubscriber_SubscriberNotFound()
        {
            var subscriberName = Guid.NewGuid().ToString();
            var subscriptionName = Guid.NewGuid().ToString();
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);

            await service.CreateUserAsync(subscriptionName);
            var result = await service.SubscribeAsync(subscriberName, subscriptionName);

            Assert.Equal(SubscribeStatus.SubscriberNotFound, result.Status);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Subscribe_NotExistsSubscription_SubscriptionNotFound()
        {
            var subscriberName = Guid.NewGuid().ToString();
            var subscriptionName = Guid.NewGuid().ToString();
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);

            await service.CreateUserAsync(subscriberName);
            var result = await service.SubscribeAsync(subscriberName, subscriptionName);

            Assert.Equal(SubscribeStatus.SubscriptionNotFound, result.Status);
            Assert.Null(result.Value);
        }
    }
}