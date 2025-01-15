using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializerService _serializerService;

        public BasketRepository(IDistributedCache redisCacheService, ISerializerService serializerService)
        {
            _redisCacheService = redisCacheService;
            _serializerService = serializerService;
        }

        public async Task<Cart?> GetBasketByUserNameAsync(string userName)
        {
            var cart = await _redisCacheService.GetStringAsync(userName);
            return string.IsNullOrEmpty(cart) ? null : _serializerService.Deserialize<Cart>(cart);
        }

        public async Task<Cart> UpdateBasketAsync(Cart cart, DistributedCacheEntryOptions options)
        {
            var jsonCart = _serializerService.Serialize(cart);

            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName, jsonCart, options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, jsonCart);
            }
            return await GetBasketByUserNameAsync(cart.UserName);
        }

        public async Task DeleteBasketFromUserNameAsync(string userName)
        {
            await _redisCacheService.RemoveAsync(userName);
        }
    }
}
