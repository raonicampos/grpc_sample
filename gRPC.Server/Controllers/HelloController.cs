using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Hello;

namespace gRPC.Server.Controllers
{
    public class HelloController : HelloWorld.HelloWorldBase
    {
        public override Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse
            {
                Message = $"Hello {request.Name}",
                Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
            });
        }

        public override async Task<HelloResponse> HelloRequestStream(IAsyncStreamReader<HelloRequest> requestStream,
            ServerCallContext context)
        {
            var name = string.Empty;
            var requests = 0;
            while (!context.CancellationToken.IsCancellationRequested && await requestStream.MoveNext())
            {
                name = requestStream.Current.Name;
                requests++;
            }

            return new HelloResponse
            {
                Message = $"Hello {name}, received {requests} requests",
                Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
            };
        }

        public override async Task HelloResponseStream(HelloCountRequest request,
            IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
            var responseCount = 0;
            while (responseCount < request.ResponseCount && !context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new HelloResponse
                {
                    Message = $"Hello {request.Name}! Response number {++responseCount}.",
                    Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
                });
                await Task.Delay(1000);
            }
        }

        public override async Task HelloBiStream(IAsyncStreamReader<HelloRequest> requestStream,
            IServerStreamWriter<HelloResponse> responseStream,
            ServerCallContext context)
        {
            var requestTask = HandleBiRequestStream(requestStream, context);
            var responseTask = HandleBiResponseStream(responseStream, context);
            await Task.WhenAll(requestTask, responseTask);
        }

        private async Task HandleBiResponseStream(IAsyncStreamWriter<HelloResponse> responseStream,
            ServerCallContext context)
        {
            var sent = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new HelloResponse
                {
                    Message = $"Hello! Already sent {++sent} messages to you!",
                    Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
                });
                await Task.Delay(1000);
            }
        }

        private async Task HandleBiRequestStream(IAsyncStreamReader<HelloRequest> asyncStreamReader,
            ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested && await asyncStreamReader.MoveNext())
            {
            }
        }

        public override Task<Empty> Unimplemented(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}