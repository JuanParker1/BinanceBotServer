using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Application/user events controller
    /// </summary>
    [Route("api/events")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }
        
        /// <summary>
        /// Gets user events
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="days"> Requested interval in days </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Pagination container with user events </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserEventsAsync([Range(1, int.MaxValue)] int idUser, 
            [Range(0, int.MaxValue)] int days = 1, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var eventDtos = await _eventService.GetAllAsync(idUser, days,
                token);

            return Ok(eventDtos);
        }
    }
}