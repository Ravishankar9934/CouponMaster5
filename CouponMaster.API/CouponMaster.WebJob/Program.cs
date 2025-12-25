using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CouponMaster.WebJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            // 1. Configure WebJobs
            builder.ConfigureWebJobs(b =>
            {
                // b.AddAzureStorageCoreServices(); // Required for triggers
                b.AddTimers(); // Enables [TimerTrigger]
            });

            // 2. Configure Logging (So we can see output in Console)
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            // 3. Run
            var host = builder.Build();
            using (host)
            {
                Console.WriteLine("WebJob Host Starting...");
                await host.RunAsync();
            }
        }
    }
}