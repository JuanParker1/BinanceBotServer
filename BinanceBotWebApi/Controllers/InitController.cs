using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BinanceBotWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitController : ControllerBase
    {
        private readonly ILogger<int> _logger;

        public InitController(ILogger<int> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public int Get()
        {
            return 1;
        }
    }
}