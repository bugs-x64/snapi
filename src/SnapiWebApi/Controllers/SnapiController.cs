using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SnapiCore.Services;

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
        public IActionResult GetTopPersons([FromQuery] int maxCount = 100)
        {
            try
            {
                var result =  _service.GetTopPersons(maxCount);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int) HttpStatusCode.InternalServerError, errorMessage);
            }
        }
    }
}