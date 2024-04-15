using Grpc.Net.Client;
using System;
using Serilog;

namespace gRPC.Tester
{
    class Program
    {
        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                              .WriteTo.Console()
                              .MinimumLevel.Debug()
                              .CreateLogger();

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress(
                "https://localhost:7112",
                new GrpcChannelOptions { HttpHandler = httpHandler }
            );

            Tester.TesterClient client = new(channel);
            var reply = await client.SayHelloUnaryAsync(new HelloRequest { Name = "SayHelloUnaryAsync" });

            Log.Information("Greeting: {@response}", reply.Message);
        }
    }
}