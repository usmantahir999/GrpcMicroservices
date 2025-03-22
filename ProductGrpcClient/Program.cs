using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductGrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for server is running..");
            Thread.Sleep(2000);
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ProductProtoService.ProductProtoServiceClient(channel);
            //GetProductAsync
            await GetProductAsync(client);

            //GetAllProductAsync
            await GetAllProductAsync(client);
            Console.ReadLine();
        }

        private static async Task<AsyncServerStreamingCall<ProductModel>> GetAllProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //using(var clientData = client.GetAllProducts(new GetAllProductsRequest()))
            //{
            //    while(await clientData.ResponseStream.MoveNext(new CancellationToken()))
            //    {
            //        var currentProduct = clientData.ResponseStream.Current;
            //        Console.WriteLine(currentProduct);
            //    }
            //}

            //GetAllProductAsync with c#9
            Console.WriteLine("GetAllProductAsync started with c#9 !");
            var clientData = client.GetAllProducts(new GetAllProductsRequest());
            await foreach (var responseData in clientData.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{responseData}");
            }

            return clientData;
        }

        private static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("GetProductAsync started!");
            var response = await client.GetProductAsync(new GetProductRequest { ProductId = 1 });
            Console.WriteLine("GetProductAsync response" + response.ToString());
        }
    }
}
