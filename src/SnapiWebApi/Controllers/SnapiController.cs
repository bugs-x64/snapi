using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnapiCore.Data;
using SnapiCore.Data.Models;
using SnapiCore.Models;

namespace SnapiWebApi.Controllers
{
    [ApiController]
    [Route(WebConstants.RpcRoute)]
    public class SnapiController : ControllerBase
    {
        private readonly UsersService _service;
        private readonly ILogger<SnapiController> _logger;
        private const string errorMessage = "something went wrong";

        public SnapiController(ILogger<SnapiController> logger, UsersService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromQuery] string name)
        {
            try
            {
                var result = await _service.CreateUserAsync(name);

                return result.Status switch
                {
                    CreateUserStatus.Created => Ok(result.Value),
                    CreateUserStatus.AlreadyExists => BadRequest("user is already exists"),
                    CreateUserStatus.TooShortName => BadRequest("try to add more symbols)"),
                    CreateUserStatus.TooLongName => BadRequest("it's too long!"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int) HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromQuery] string yourName, [FromQuery] string yourFriendName)
        {
            try
            {
                var result = await _service.SubscribeAsync(yourName, yourFriendName);

                return result.Status switch
                {
                    SubscribeStatus.Subscribed => Ok(result.Value),
                    SubscribeStatus.AlreadySubscribed => BadRequest("AlreadySubscribed"),
                    SubscribeStatus.SubscriberNotFound => BadRequest("SubscriberNotFound"),
                    SubscribeStatus.SubscriptionNotFound => BadRequest("SubscriptionNotFound"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int) HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTopPersons([FromQuery] int maxCount = 100)
        {
            try
            {
                var result = await _service.GetTopPersonsAsync(maxCount);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int) HttpStatusCode.InternalServerError, errorMessage);
            }
        }
    }

    public class UsersService
    {
        private readonly SnapiDbContext _context;

        public UsersService(SnapiDbContext context)
        {
            _context = context;
        }


        public async Task<Result<CreateUserStatus, User>> CreateUserAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
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

        public async Task<IEnumerable<UserDto>> GetTopPersonsAsync(int maxCount)
        {
            var users = await _context.Subscribers
                .Include(x => x.To)
                .GroupBy(s => s.ToId,
                    (id, subscribers) => new
                    {
                        id,
                        subscribers = subscribers,
                        count = subscribers.Count()
                    })
                .OrderByDescending(x => x.count)
                .Take(maxCount)
                .ToArrayAsync();


            return users.Select(x => new UserDto()
            {
                Name = x.subscribers.First().To.Name,
                SubscribersCount = x.count,
                Subscribers = x.subscribers.Select(link => link.From.Name).ToArray()
            });
        }
    }

    public class UserDto
    {
        public string Name { get; set; }
        public int SubscribersCount { get; set; }
        public string[] Subscribers { get; set; }
    }

    public enum CreateUserStatus
    {
        Created,
        AlreadyExists,
        TooShortName,
        TooLongName
    }

    public enum SubscribeStatus
    {
        Subscribed,
        AlreadySubscribed,
        SubscriberNotFound,
        SubscriptionNotFound
    }
}