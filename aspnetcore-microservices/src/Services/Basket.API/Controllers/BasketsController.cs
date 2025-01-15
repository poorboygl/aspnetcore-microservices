using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;

        public BasketsController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<Cart>> GetBasketAsync(string userName)
        {
            var result = await _basketRepository.GetBasketByUserNameAsync(userName);
            return Ok(result ?? new Cart(userName));
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> UpdateBasketAsync(Cart cart)
        {
            //a cached object will be expired if it not being requested for a defined amount of time period.
            var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            var result = await _basketRepository.UpdateBasketAsync(cart, options);
            return Ok(result);
        }

        [HttpDelete("{userName}")]
        public async Task<ActionResult> DeleteBasketAsync(string userName)
        {
            await _basketRepository.DeleteBasketFromUserNameAsync(userName);
            return Accepted();
        }
    }
}
