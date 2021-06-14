using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace gRPC.Server
{
    public static class Server
    {
        private static readonly ManualResetEventSlim ShutdownEvent = new ManualResetEventSlim();

        public static async Task Start()
        {
            using var host = CreateHostBuilder().Build();
            host.Start();
            ShutdownEvent.Wait();
            await host.StopAsync();
        }

        public static void Stop()
        {
            ShutdownEvent.Set();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 50051,
                            listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                    });
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}