using System;
using System.Collections.Generic;
using System.Diagnostics;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using static System.Console;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace _07_BuiltInJsonSupport
{
    class Program
    {
        static void Main(string[] args)
        {
            ForegroundColor = ConsoleColor.White;
            WriteLine("Preparing data...");

            var data = Builder<DataPoint>.CreateListOfSize(10)
                .All()
                .With(p => p.Id = Guid.NewGuid())
                .With(p => p.Title = Faker.Identification.UkNationalInsuranceNumber())
                .With(p => p.Description = Faker.Lorem.Sentence(100))
                .With(p => p.LeftData = GenerateDataPoint(0))
                .With(p => p.RightData = GenerateDataPoint(0))
                .Build();

            var numberOfSamples = 10;
            long averageNewtonsoftSerialization = 0;
            long averageNewtonsoftDeserialization = 0;
            long averageBuiltInSerialization = 0;
            long averageBuiltInDeserialization = 0;

            for (int i = 0; i < numberOfSamples; i++)
            {
                WriteLine();

                var sw1 = Stopwatch.StartNew();
                string newtonsoftJson = JsonConvert.SerializeObject(data);
                sw1.Stop();
                ForegroundColor = ConsoleColor.Yellow;
                averageNewtonsoftSerialization += sw1.ElapsedMilliseconds;
                WriteLine($"Newtonsoft JSON serialization:   {sw1.ElapsedMilliseconds}ms");

                sw1.Restart();
                _ = JsonConvert.DeserializeObject<List<DataPoint>>(newtonsoftJson);
                sw1.Stop();
                ForegroundColor = ConsoleColor.Yellow;
                averageNewtonsoftDeserialization += sw1.ElapsedMilliseconds;
                WriteLine($"Newtonsoft JSON deserialization: {sw1.ElapsedMilliseconds}ms");

                var sw2 = Stopwatch.StartNew();
                string builtinJson = JsonSerializer.Serialize(data);
                sw2.Stop();
                ForegroundColor = ConsoleColor.Green;
                averageBuiltInSerialization += sw2.ElapsedMilliseconds;
                WriteLine($"Built-in JSON serialization:     {sw2.ElapsedMilliseconds}ms");

                sw2.Restart();
                _ = JsonSerializer.Deserialize<List<DataPoint>>(builtinJson);
                sw2.Stop();
                ForegroundColor = ConsoleColor.Green;
                averageBuiltInDeserialization += sw2.ElapsedMilliseconds;
                WriteLine($"Built-in JSON deserialization:   {sw2.ElapsedMilliseconds}ms");
            }

            WriteLine();

            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"Newtonsoft JSON serialization:   {averageNewtonsoftSerialization / numberOfSamples}ms");
            WriteLine($"Newtonsoft JSON deserialization: {averageNewtonsoftDeserialization / numberOfSamples}ms");

            ForegroundColor = ConsoleColor.Green;
            WriteLine($"Built-in JSON serialization:     {averageBuiltInSerialization / numberOfSamples}ms");
            WriteLine($"Built-in JSON deserialization:   {averageBuiltInDeserialization / numberOfSamples}ms");
        }

        static DataPoint GenerateDataPoint(int level)
        {
            if (level < 10)
            {
                return new DataPoint
                {
                    Id = Guid.NewGuid(),
                    Title = Faker.Identification.UkNationalInsuranceNumber(),
                    Description = Faker.Lorem.Sentence(10),
                    LeftData = GenerateDataPoint(level + 1),
                    RightData = GenerateDataPoint(level + 1)
                };
            }

            return null;
        }
    }
}