using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;


    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    #region CRUD
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync()
    {
        var result = await _productRepository.GetProductsAsync();
        return Ok(result);
    }
    #endregion

}