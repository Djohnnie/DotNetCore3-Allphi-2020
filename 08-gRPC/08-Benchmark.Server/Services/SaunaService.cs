using _08_Benchmark.Proto;
using _08_Benchmark.Server.Managers;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace _08_Benchmark.Server.Services
{
    public class SaunaService : Sauna.SaunaBase
    {
        private readonly SaunaManager _saunaManager;
        private readonly ILogger<SaunaService> _logger;

        public SaunaService(
            SaunaManager saunaManager,
            ILogger<SaunaService> logger)
        {
            _saunaManager = saunaManager;
            _logger = logger;
        }

        public override Task<SaunaResponse> FetchCurrentState(SaunaRequest request, ServerCallContext context)
        {
            return _saunaManager.FetchCurrentState(request);
        }
    }
}