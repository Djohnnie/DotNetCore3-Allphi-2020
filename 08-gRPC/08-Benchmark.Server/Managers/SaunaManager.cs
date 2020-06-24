using _08_Benchmark.Proto;
using System;
using System.Threading.Tasks;

namespace _08_Benchmark.Server.Managers
{
    public class SaunaManager
    {
        private readonly Random _randomGenerator = new Random();
        private SaunaResponse _cachedResponse = null;

        public Task<SaunaResponse> FetchCurrentState(SaunaRequest request)
        {
            if(_cachedResponse == null)
            {
                _cachedResponse = CreateSaunaResponse(0, request);
            }

            return Task.FromResult(_cachedResponse);
        }

        private SaunaResponse CreateSaunaResponse(int counter, SaunaRequest request)
        {
            if (counter > 30)
            {
                return null;
            }

            return new SaunaResponse
            {
                TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsDrySauna = _randomGenerator.Next(0, 2) == 1,
                IsInfraRed = _randomGenerator.Next(0, 2) == 1,
                Temperature = GetTemperature(request.TemperatureUnit),
                Description = $"{typeof(SaunaManager)}",
                Deeper = CreateSaunaResponse(counter + 1, request)
            };
        }

        private int GetTemperature(string unit)
        {
            int celsius = _randomGenerator.Next(19, 110) + 1;
            return unit == "F" ? celsius * 9 / 5 + 32 : celsius;
        }
    }
}