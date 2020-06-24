using _08_Benchmark.Proto;
using _08_Benchmark.Server.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace _08_Benchmark.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SaunaController : ControllerBase
    {
        private readonly SaunaManager _saunaManager;
        private readonly ILogger<SaunaController> _logger;

        public SaunaController(
            SaunaManager saunaManager,
            ILogger<SaunaController> logger)
        {
            _saunaManager = saunaManager;
            _logger = logger;
        }

        [HttpGet]
        public Task<SaunaResponse> Get()
        {
            return _saunaManager.FetchCurrentState(new SaunaRequest { TemperatureUnit = "C" });
        }
    }
}