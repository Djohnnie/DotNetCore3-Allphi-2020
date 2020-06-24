using _08_Benchmark.Proto;
using Grpc.Net.Client;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Console;

namespace _08_Benchmark.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(5000);

            var grpcChannel = GrpcChannel.ForAddress("https://localhost:5001");
            var restClient = new RestClient("https://localhost:5001");
            var grpcClient = new Sauna.SaunaClient(grpcChannel);
            var grpcRequest = new SaunaRequest { TemperatureUnit = "C" };
            var restRequest = new RestRequest("sauna", Method.GET);
            await grpcClient.FetchCurrentStateAsync(grpcRequest);
            await restClient.GetAsync<SaunaResponse>(restRequest);

            var numberOfSamples = 10;
            long totalGrpcMilliseconds = 0;
            long totalRestMilliseconds = 0;

            for (int i = 0; i < numberOfSamples; i++)
            {
                WriteLine();

                var sw1 = Stopwatch.StartNew();
                var grpcResponse = await grpcClient.FetchCurrentStateAsync(grpcRequest);
                sw1.Stop();
                ForegroundColor = ConsoleColor.Green;
                totalGrpcMilliseconds += sw1.ElapsedMilliseconds;
                WriteLine($"gRPC: {sw1.ElapsedMilliseconds}ms");

                var sw2 = Stopwatch.StartNew();
                var restResponse = await restClient.GetAsync<SaunaResponse>(restRequest);
                sw2.Stop();
                ForegroundColor = ConsoleColor.Yellow;
                totalRestMilliseconds += sw2.ElapsedMilliseconds;
                WriteLine($"REST: {sw2.ElapsedMilliseconds}ms");

            }

            WriteLine();
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Total gRPC:   {totalGrpcMilliseconds}ms");
            WriteLine($"Average gRPC: {totalGrpcMilliseconds / numberOfSamples}ms");
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"Total REST:   {totalRestMilliseconds}ms");
            WriteLine($"Average REST: {totalRestMilliseconds / numberOfSamples}ms");
        }


    }
}
