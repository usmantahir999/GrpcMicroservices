using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Protos;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Services
{
    public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
    {
        private readonly ShoppingCartContext _context;
        private readonly ILogger<ShoppingCartService> _logger;
        private readonly IMapper _mapper;
        public ShoppingCartService(ShoppingCartContext context, ILogger<ShoppingCartService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
        {
            var shoppingCart = await _context.ShoppingCart.FirstOrDefaultAsync(s=>s.UserName ==request.Username);
            if(shoppingCart == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart with username = {request.Username} not found"));
            }
            var shoppingCartModel = new ShoppingCartModel();
            return shoppingCartModel;
        }
    }
}
