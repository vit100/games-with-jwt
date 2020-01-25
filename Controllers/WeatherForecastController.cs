using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme
            //, Roles = "admin"
            ,Policy= "mustBeInScope_Something"
            )]
        public IEnumerable<WeatherForecast> Get()
        {
            var u = User;
            var c = HttpContext;
            var roles = User.IsInRole("openid b2c-p2p-v1-0ff75e33-5086-40d8-acb2-85d6a4a07698 b2c-profile-v1-93a6ef22-0aa6-43f1-9624-f33ee8022e49 b2c-external-transfers-v1-8c1d23d5-c8eb-475e-ad16-8c69aba70a68 b2c-check-deposit-v1-e1d04baf-453e-450e-8956-c225e49afe67 retail-psd2-stet-aisp-1d3-8f797f8c-e71e-11e8-9f32-f2801f1b9fd1 b2c-internal-transfers-v1-fbc24c2a-46ad-4cc2-8e42-2a56d761261d b2c-account-v1-fc77362a-c2ee-4b23-b20e-5621249eb7a4");
            

            var x = ControllerContext.ActionDescriptor;
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        public async Task<ActionResult<string>> GetWithGoogleAuth()
        {
            return "Google auth works!" + JsonConvert.SerializeObject(new
            {
                Claims = User.Claims,
                Identity = User.Identity
            }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented

            });
        }

        [Route("[action]")]
        public async Task Signout()
        {
            HttpContext.GetIdentityServerBaseUrl();
           await HttpContext.SignOutAsync();
        }
    }
}
