using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

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
        /// <response code="200"> User info </response>
        /// <response code="403"> Invalid login or password </response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthUserInfoDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthDto authDto, 
            CancellationToken token = default)
        {
            var authUserInfo = await _authService.LoginAsync(authDto.Login,
                authDto.Password, token);

            if (authUserInfo is null)
                return Forbid();

            return Ok(authUserInfo);
        }
        
        /// <summary>
        /// Extend token validity
        /// </summary>
        /// <returns code="200"> New token </returns>
        [HttpGet("refresh")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public ActionResult Refresh() // TODO: Добавить обновление токена
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
        /// <response code="400"> User already exists </response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto,
            CancellationToken token = default)
        {
            var isNewUser = await _authService.RegisterAsync(registerDto, token);

            if (!isNewUser) 
                return BadRequest("User with such login already exists");
            
            var authUserInfo = await _authService.LoginAsync(registerDto.Login,
                registerDto.Password, token);
            
            return Ok(authUserInfo);
        }
        
        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="changePasswordDto"> User password info </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ок </returns>
        /// <response code="403"> Wrong user id or permissions </response>
        [HttpPut("changePassword")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto, 
            CancellationToken token = default)
        {
            if (User.GetUserId() == changePasswordDto.IdUser || User.IsInRole("Administrator"))
                return Forbid();

            var code = await _authService.ChangePasswordAsync(changePasswordDto, token);
            
            return code switch
            {
                0 => Ok(),
                -1 => BadRequest("User does not exist"),
                -2 => BadRequest("Old password is incorrect"),
                _ => BadRequest(),
            };
        }
    }
}