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
    /// Http requests controller
    /// </summary>
    [Route("api/requests")]
    [ApiController]
    [Authorize]
    public class RequestTrackerController : ControllerBase
    {
        private readonly IRequestTrackerService _requestService;

        public RequestTrackerController(IRequestTrackerService requestService)
        {
            _requestService = requestService;
        }
        
        /// <summary>
        /// Get requested amount of last requests
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="take"> Amount of requested entities </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Requested amount of last requests </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetRequestsAsync([FromQuery][Range(1, int.MaxValue)] int idUser, 
            [Range(1, int.MaxValue)] int take = 10, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var requests = await _requestService.GetUserRequestsAsync(idUser,
                take, token);
            
            return Ok(requests);
        }
    }
}