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
        /// <param name="auth"></param>
        /// <param name="token"> Task cancellation token </param>
        /// <response code="200"> New token </response>
        /// <response code="400"> Invalid login or password т</response>
        [HttpPost("login")]
        [AllowAnonymous]
        //[SwaggerOperation(OperationId = "login")]
        [ProducesResponseType(typeof(AuthUserInfoDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthDto auth, 
            CancellationToken token = default)
        {
            var userToken = await _authService.LoginAsync(auth.Login,
                auth.Password, token);

            if (userToken is null)
                return Forbid();

            return Ok(userToken);
        }
        
        /// <summary>
        /// Extend token validity
        /// </summary>
        /// <returns code="200"> New token </returns>
        [HttpGet("refresh")]
        [Authorize]
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
        /// <returns code="200"> Ок </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto,
            CancellationToken token = default)
        {
            var isNewUser = await _authService.RegisterAsync(registerDto, token);

            if (!isNewUser) 
                return Forbid();
            
            var userToken = await _authService.LoginAsync(registerDto.Login,
                registerDto.Password, token);
            
            return Ok(userToken);
        }
        
        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="newPassword"> New user password </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ок </returns>
        [HttpPut("{idUser}/changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute]int idUser, 
            [FromBody]string newPassword, CancellationToken token = default)
        {
            if (User.GetUserId() == idUser || User.IsInRole("Administrator"))
                return Forbid();

            var code = await _authService.ChangePasswordAsync(idUser, newPassword, token);
            
            return code switch
            {
                0 => Ok(),
                -1 => BadRequest("User does not exist"),
                _ => BadRequest(),
            };
        }
    }
}