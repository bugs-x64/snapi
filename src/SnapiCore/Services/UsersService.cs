using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SnapiCore.Data;
using SnapiCore.Data.Models;
using SnapiCore.Models;

namespace SnapiCore.Services
{
    public class UsersService
    {
        private readonly SnapiDbContext _context;

        public UsersService(SnapiDbContext context)
        {
            _context = context;
        }


        public async Task<Result<CreateUserStatus, User>> CreateUserAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length <= 3)
                return (CreateUserStatus.TooShortName, null);

            if (name.Length > 64)
                return (CreateUserStatus.TooLongName, null);

            var isUserExists = await _context.Users.AsQueryable().AnyAsync(x => x.IndexName == ToIndexName(name));
            if (isUserExists)
            {
                return (CreateUserStatus.AlreadyExists, null);
            }

            var user = new User()
            {
                Name = name,
                IndexName = ToIndexName(name)
            };

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return (CreateUserStatus.Created, user);
        }

        private static string ToIndexName(string name)
        {
            return name.ToLowerInvariant().Replace(" ", "");
        }


        public async Task<Result<SubscribeStatus, SubscriberLink>> SubscribeAsync(string subscriberName, string subscriptionName)
        {
            var subscriber = await _context.Users.AsQueryable()
                .FirstOrDefaultAsync(x => x.IndexName == ToIndexName(subscriberName));
            if (subscriber is null)
                return (SubscribeStatus.SubscriberNotFound, null);

            var subscription = await _context.Users.AsQueryable()
                .FirstOrDefaultAsync(x => x.IndexName == ToIndexName(subscriptionName));
            if (subscription is null)
                return (SubscribeStatus.SubscriptionNotFound, null);

            var link = await _context.Subscribers.AsQueryable()
                .FirstOrDefaultAsync(x => x.FromId == subscriber.Id && x.ToId == subscription.Id);
            if (link is not null)
                return (SubscribeStatus.AlreadySubscribed, null);


            link = new SubscriberLink()
            {
                FromId = subscriber.Id,
                ToId = subscription.Id
            };

            await _context.Subscribers.AddAsync(link);

            await _context.SaveChangesAsync();

            return (SubscribeStatus.Subscribed, link);
        }

        public IEnumerable<UserDto> GetTopPersons(int maxCount)
        {
            //Какая-то бага EfCore SQLite, из-за которой GroupBy не хочет работать.
            //Так как база небольшая, можно и в памяти выполнить
            var users = _context.Subscribers
                .Include(x=>x.To)
                .Include(x=>x.From)
                .AsEnumerable()
                .GroupBy(s => s.ToId,
                    (id, subscribers) => new
                    {
                        id,
                        subscribers = subscribers,
                        count = subscribers.Count()
                    })
                .OrderByDescending(x => x.count)
                .Take(maxCount);


            return users.Select(x => new UserDto()
            {
                Name = x.subscribers.First().To.Name,
                SubscribersCount = x.count,
                Subscribers = x.subscribers.Select(link => link.From.Name).ToArray()
            });
        }
    }
}