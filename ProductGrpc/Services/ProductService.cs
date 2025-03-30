using AutoMapper;
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
        private readonly IMapper _mapper;
        public ProductService(ProductsContext context, ILogger<ProductService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _context.Product.FindAsync(request.ProductId)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"Product with Id = {request.ProductId} not found"));
            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }

        public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
        {
            var productList = await _context.Product.ToListAsync();
            foreach (var product in productList) 
            {
                var productModel = _mapper.Map<ProductModel>(product);
                await responseStream.WriteAsync(productModel);
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }


        public override async Task<ProductModel> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);
            bool isExist = await _context.Product.AnyAsync(x => x.ProductId == request.Product.ProductId);
            if (!isExist) {
                throw new RpcException(new Status(StatusCode.NotFound, $"Product with Id = {request.Product.ProductId} not found"));
            }
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                throw;
            }
            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }

        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            var product = await _context.Product.FirstAsync(x => x.ProductId == request.ProductId);
            if (product == null) {
                throw new RpcException(new Status(StatusCode.NotFound, $"Product with Id = {request.ProductId} not found"));
            }
            _context.Product.Remove(product);
            var deleteCount = await _context.SaveChangesAsync();
            var response = new DeleteProductResponse
            {
                Success = deleteCount > 0
            };
            return response;
        }

        public override async Task<InsertBulkProductResponse> InsertBulkProduct(IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var product = _mapper.Map<Product>(requestStream.Current);
                _context.Product.Add(product);
            }
            var insertCount = await _context.SaveChangesAsync();
            return new InsertBulkProductResponse
            {
                Success = insertCount > 0,
                InsertCount = insertCount
            };
        }
    }
} 
