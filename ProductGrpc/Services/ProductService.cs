using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductGrpc.Data;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using System.Threading.Tasks;

namespace ProductGrpc.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductsContext _context;
        private readonly ILogger<ProductService> _logger;
        public ProductService( ProductsContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _context.Product.FindAsync(request.ProductId);
            if (product == null) {
                throw new System.Exception();
            }
            var productModel = new ProductModel
            {
                ProductId = request.ProductId,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                Status = Protos.ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            };
            return productModel;
        }

        public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
        {
            var productList = await _context.Product.ToListAsync();
            foreach (var product in productList) 
            {
                var productModel = new ProductModel
                {
                    ProductId = product.ProductId,
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Status = Protos.ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
                };

                await responseStream.WriteAsync(productModel);
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = new Product
            {
                ProductId = request.Product.ProductId,
                Name = request.Product.Name,
                Price = request.Product.Price,
                Description = request.Product.Description,
                Status = Models.ProductStatus.INSTOCK,
                CreatedTime = request.Product.CreatedTime.ToDateTime()
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            var productModel = new ProductModel
            {
                ProductId = product.ProductId,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                Status = Protos.ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            };
            return productModel;
        }
    }
} 
