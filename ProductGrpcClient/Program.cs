using Google.Protobuf.WellKnownTypes;
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
            await GetProductAsync(client);
            await GetAllProductAsync(client);
            await AddProductAsync(client);
            await UpdateProductAsync(client);
            await DeleteProductAsync(client);
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

        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("AddProductAsync started!");
            var addProductResponse = await client.AddProductAsync(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Red",
                    Description = "New Red Phone Mi10T",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });
            Console.WriteLine("AddProductAsync response" + addProductResponse.ToString());
        }

        private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("UpdateProductAsync started!");
            var updateProductResponse = await client.UpdateProductAsync(new UpdateProductRequest
            {
                Product = new ProductModel
                {
                    ProductId = 1,
                    Name = "Red",
                    Description = "New Red Phone Mi10T",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });
            Console.WriteLine("UpdateProductAsync response" + updateProductResponse.ToString());
        }

        private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("DeleteProductAsync started!");
            var deleteProductResponse = await client.DeleteProductAsync(new DeleteProductRequest { ProductId = 3 });
            Console.WriteLine("DeleteProductAsync response" + deleteProductResponse.Success.ToString());
        }

        
    }
}
