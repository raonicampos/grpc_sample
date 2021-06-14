using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Hello;

namespace gRPC.Client
{
    internal static class HelloProgram
    {
        
        public static async Task Main()
        {
            var channel = new Channel("localhost:50051", ChannelCredentials.Insecure);
            var client = new HelloWorld.HelloWorldClient(channel);
            
            await SimpleHello(client);

            await channel.ShutdownAsync();
        }

        private static async Task SimpleHello(HelloWorld.HelloWorldClient client)
        {
            var requests = 10;
            while (requests-- > 0)
            {
                var echoResponse = await client.HelloAsync(new HelloRequest
                {
                    Name = Guid.NewGuid().ToString()
                });
                Console.WriteLine("Response: " + echoResponse.Message);
                await Task.Delay(1000);
            }
        }
    }
}