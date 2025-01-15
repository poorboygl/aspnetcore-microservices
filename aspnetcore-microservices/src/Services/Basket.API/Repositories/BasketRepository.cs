using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger; // Đè lên logger của Microsoft

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializerService _serializerService;
        private readonly ILogger _logger;

        public BasketRepository(IDistributedCache redisCacheService, ISerializerService serializerService, ILogger logger)
        {
            _redisCacheService = redisCacheService;
            _serializerService = serializerService;
            _logger = logger;
        }

        public async Task<Cart> GetBasketByUserNameAsync(string userName)
        {
            _logger.Information($"BEGIN: GetBaskByUserName {userName}");
            var basket = await _redisCacheService.GetStringAsync(userName);
            _logger.Information($"END: GetBaskByUserName {userName}");

            return string.IsNullOrEmpty(basket) ? null : _serializerService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasketAsync(Cart cart, DistributedCacheEntryOptions options)
        {
            _logger.Information($"BEGIN: UpdateBasket for {cart.UserName}");
            var jsonCart = _serializerService.Serialize(cart);

            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, jsonCart, options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, jsonCart);
            }
            _logger.Information($"END: UpdateBasket for {cart.UserName}");
            return await GetBasketByUserNameAsync(cart.UserName);
        }

        public async Task<bool> DeleteBasketFromUserNameAsync(string userName)
        {
            try
            {
                _logger.Information($"BEGIN: DeteleBasketFromUserName {userName}");
                await _redisCacheService.RemoveAsync(userName);
                _logger.Information($"END: DeteleBasketFromUserName {userName}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("DeteleBasketFromUserName: " + ex.Message);
                throw;
            }
        }
    }
}
