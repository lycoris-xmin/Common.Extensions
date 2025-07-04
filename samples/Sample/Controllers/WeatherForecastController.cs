using Lycoris.Common.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers
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
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var result = IPAddressHelper.Search("110.89.30.188");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }

    public class GithubPutFileRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubCommitter Committer { get; set; } = new GithubCommitter();

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }
    }

    public class GithubCommitter
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }
    }
}