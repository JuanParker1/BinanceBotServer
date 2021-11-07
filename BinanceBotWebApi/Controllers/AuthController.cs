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
        [ProducesResponseType(typeof(UserTokenDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthDto auth, 
            CancellationToken token = default)
        {
            var userToken = await _authService.LoginAsync(auth.Login,
                auth.Password, token).ConfigureAwait(false);

            if (userToken is null)
                Forbid();

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
        /// Register
        /// </summary>
        /// <param name="user"> New user info </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ок </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(UserDto user,
            CancellationToken token = default)
        {
            var code = await _authService.RegisterAsync(user, token)
                .ConfigureAwait(false);
            
            return code switch
            {
                0 => Ok(),
                -1 => BadRequest("Login should be more than 3 characters."),
                -2 => BadRequest("Password should be more than 3 characters."),
                -3 => BadRequest("Email should not be more than 255 characters."),
                -4 => BadRequest("User already exists."),
                _ => BadRequest(),
            };
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
        public async Task<IActionResult> ChangePasswordAsync([FromRoute]int idUser, [FromBody]string newPassword,
            CancellationToken token = default)
        {
            if (User.GetUserId() == idUser || User.IsInRole("Administrator"))
                return Forbid($"You must be an Administrator or user with id:{idUser}.");

            var code = await _authService.ChangePasswordAsync(idUser, newPassword, token)
                .ConfigureAwait(false);
            
            return code switch
            {
                0 => Ok(),
                -1 => BadRequest("User does not exist"),
                _ => BadRequest(),
            };
        }
    }
}