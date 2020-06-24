using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Console;

namespace _06_HardwareIntrinsics
{
    class Program
    {
        static void Main(string[] args)
        {
            ForegroundColor = ConsoleColor.White;
            WriteLine("Getting two billion integers...");
            var enumerable = Enumerable.Repeat(1, 2146435071).ToArray();
            var source = new ReadOnlySpan<int>(enumerable);

            int numberOfSamples = 5;
            long totalMillisecondsForARegularSum = 0;
            long totalMillisecondsForASimdSum = 0;
            long totalMillisecondsForAnIntrinsicsSum = 0;
            int result;

            for (int i = 0; i < numberOfSamples; i++)
            {
                WriteLine();
                ForegroundColor = ConsoleColor.Red;
                Write("Calculating a regular sum...                          ");
                var sw1 = Stopwatch.StartNew();
                result = Sum(source);
                sw1.Stop();
                totalMillisecondsForARegularSum += sw1.ElapsedMilliseconds;
                WriteLine($"{result} ({sw1.ElapsedMilliseconds}ms)");

                ForegroundColor = ConsoleColor.Yellow;
                Write("Calculating a sum with SIMD support...                ");
                var sw2 = Stopwatch.StartNew();
                result = SumSimd(source);
                sw2.Stop();
                totalMillisecondsForASimdSum += sw2.ElapsedMilliseconds;
                WriteLine($"{result} ({sw2.ElapsedMilliseconds}ms)");

                ForegroundColor = ConsoleColor.Green;
                Write("Calculating a sum with Hardware Intrinsics support... ");
                var sw3 = Stopwatch.StartNew();
                result = SumVectorized(source);
                sw3.Stop();
                totalMillisecondsForAnIntrinsicsSum += sw3.ElapsedMilliseconds;
                WriteLine($"{result} ({sw3.ElapsedMilliseconds}ms)");
            }

            WriteLine();
            ForegroundColor = ConsoleColor.Red;
            Write("A regular sum                          ");
            WriteLine($"{totalMillisecondsForARegularSum / numberOfSamples}ms");

            ForegroundColor = ConsoleColor.Yellow;
            Write("A sum with SIMD support                ");
            WriteLine($"{totalMillisecondsForASimdSum / numberOfSamples}ms");

            ForegroundColor = ConsoleColor.Green;
            Write("A sum with Hardware Intrinsics support ");
            WriteLine($"{totalMillisecondsForAnIntrinsicsSum / numberOfSamples}ms");
        }

        static int Sum(ReadOnlySpan<int> source)
        {
            int result = 0;

            foreach (var number in source)
            {
                result += number;
            }

            return result;
        }

        static int SumSimd(ReadOnlySpan<int> source)
        {
            int result = 0;
            Vector<int> resultVector = Vector<int>.Zero;

            int i = 0;
            int lastBlockIndex = source.Length - source.Length % Vector<int>.Count;

            while (i < lastBlockIndex)
            {
                resultVector += new Vector<int>(source.Slice(i));
                i += Vector<int>.Count;
            }

            for (int n = 0; n < Vector<int>.Count; n++)
            {
                result += resultVector[n];
            }

            while (i < source.Length)
            {
                result += source[i];
                i += 1;
            }

            return result;
        }

        static int SumVectorized(ReadOnlySpan<int> source)
        {
            return Sse2.IsSupported ? SumVectorizedSse(source) : SumSimd(source);
        }

        static unsafe int SumVectorizedSse(ReadOnlySpan<int> source)
        {
            int result;

            fixed (int* sourcePointer = source)
            {
                Vector128<int> resultVector = Vector128<int>.Zero;

                int i = 0;
                int lastBlockIndex = source.Length - source.Length % 4;

                while (i < lastBlockIndex)
                {
                    resultVector = Sse2.Add(resultVector, Sse2.LoadVector128(sourcePointer + i));
                    i += 4;
                }

                if (Ssse3.IsSupported)
                {
                    resultVector = Ssse3.HorizontalAdd(resultVector, resultVector);
                    resultVector = Ssse3.HorizontalAdd(resultVector, resultVector);
                }
                else
                {
                    resultVector = Sse2.Add(resultVector, Sse2.Shuffle(resultVector, 0x4E));
                    resultVector = Sse2.Add(resultVector, Sse2.Shuffle(resultVector, 0xB1));
                }
                result = resultVector.ToScalar();

                while (i < source.Length)
                {
                    result += sourcePointer[i];
                    i += 1;
                }
            }

            return result;
        }
    }
}