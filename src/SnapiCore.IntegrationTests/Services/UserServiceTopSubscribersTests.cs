using System;
using System.Linq;
using System.Threading.Tasks;
using SnapiCore.Data;
using SnapiCore.Services;
using Xunit;

namespace SnapiCore.IntegrationTests.Services
{
    public class UserServiceTopSubscribersTests
    {
        public async Task Fill(UsersService service, string top)
        {
            await service.CreateUserAsync(top);
            var count = service.GetTopPersons(1).FirstOrDefault()?.SubscribersCount ?? 0;
            foreach (var _ in Enumerable.Range(0, count + 1))
            {
                var user = Guid.NewGuid().ToString();
                await service.CreateUserAsync(user);

                await service.SubscribeAsync(user, top);
            }
        }
        [Fact]
        public async Task TopSubscribers_UserWithMaximumSubscribers_UserInTop()
        {
            var topUser = Guid.NewGuid().ToString();
            await using var context = DbContextFactory.Get();
            var service = new UsersService(context);
            await Fill(service, topUser);

            var result = service.GetTopPersons(2);

            var userDto = result.FirstOrDefault();
            Assert.NotNull(userDto);
            Assert.Equal(topUser, userDto.Name);
            Assert.NotEqual(userDto.Name, result.Skip(1).FirstOrDefault()?.Name);
        }
    }
}