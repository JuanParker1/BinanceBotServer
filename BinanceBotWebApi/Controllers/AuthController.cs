using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotWebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// User authentication
        /// </summary>
        /// <param name="authDto"></param>
        /// <param name="token"> Task cancellation token </param>
        /// <response code="200"> User token </response>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Invalid login or password </response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthTokenDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthDto authDto, 
            CancellationToken token = default)
        {
            var authToken = await _authService.LoginAsync(authDto.Login,
                authDto.Password, token);

            if (authToken is null)
                return Forbid();

            return Ok(authToken);
        }
        
        /// <summary>
        /// Extend token validity
        /// </summary>
        /// <returns code="200"> New token </returns>
        [HttpGet("refresh")]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.OK)]
        public ActionResult Refresh()
        {
            var newToken = _authService.Refresh(User);
            return Ok(newToken);
        }

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="registerDto"> New user info </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> User already exists </response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthTokenDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto,
            CancellationToken token = default)
        {
            var isNewUser = await _authService.RegisterAsync(registerDto, token);

            if (!isNewUser) 
                return Forbid();
            
            var authToken = await _authService.LoginAsync(registerDto.Login,
                registerDto.Password, token);
            
            return Ok(authToken);
        }
    }
}