using System;
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
        private readonly IEventsService _eventsService;

        public EventsController(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }
        
        /// <summary>
        /// Gets user events
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="isUnreadRequested"> Return only unread events (yes/no) </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Collection of user events </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserEventsAsync([Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart, DateTime intervalEnd, bool isUnreadRequested = false, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var eventDtos = await _eventsService.GetAllAsync(idUser, 
                isUnreadRequested, intervalStart, intervalEnd, token);

            return Ok(eventDtos);
        }
        
        /// <summary>
        /// Mark requested events as read by user
        /// </summary>
        /// <param name="idsDto"> Events ids </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPut]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> MarkAsReadAsync(GenericCollectionDto<int> idsDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idsDto.IdUser)
                return Forbid();

            var result = await _eventsService.MarkAsReadAsync(idsDto, 
                token);

            return Ok(result);
        }
    }
}