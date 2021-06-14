using System.Threading.Tasks;

namespace gRPC.Server
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await Server.Start();
        }
    }
}