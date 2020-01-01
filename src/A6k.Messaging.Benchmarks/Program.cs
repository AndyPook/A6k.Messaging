using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace A6k.Messaging.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("cancelled...");
                cts.Cancel();
                e.Cancel = true;
            };

            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            //await Run(cts.Token);
            BenchmarkRunner.Run<MessageSerializationBenchmarks>(
                ManualConfig
                    .Create(DefaultConfig.Instance)
                    .With(ConfigOptions.DisableOptimizationsValidator)
            );

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static async Task Run(CancellationToken cancellationToken)
        {
            var b = new MessagePumpBenchmarks();
            b.Setup();

            for (int i = 0; i < 100_000_000 && !cancellationToken.IsCancellationRequested; i++)
            {
                if (i % 1000 == 0)
                    Console.Write(i + "\n");
                await b.FakePump();
            }
        }
    }
}
