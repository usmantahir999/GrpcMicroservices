using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ProductWorkerService
{
    public class ProductFactory
    {
        private readonly ILogger<ProductFactory> _logger;
        private readonly IConfiguration _configuration;

        public ProductFactory(IConfiguration configuration, ILogger<ProductFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<AddProductRequest> Generate()
        {
            var productName = _configuration.GetValue<string>("WorkerService:ProductName") + DateTimeOffset.Now;
            var product = new ProductModel
            {
                Name = productName,
                Description = $"{productName}_Description",
                Price = new Random().Next(1000),
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
            };
            var addProductRequest = new AddProductRequest
            {
                Product = product
            };
            return Task.FromResult(addProductRequest);
        }
    }
}
