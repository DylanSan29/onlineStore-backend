using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("api/products/{productId:int}/images")]
public sealed class ProductImagesController(ProductImageService images) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int productId) => Ok(await images.GetAsync(productId));

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Add(int productId, ProductImageRequest request)
    {
        var image = await images.AddAsync(productId, request);
        return image is null ? NotFound(new { message = "Product not found" }) : Ok(image);
    }

    [HttpDelete("{imageId:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int productId, int imageId) =>
        await images.DeleteAsync(productId, imageId) ? NoContent() : NotFound();
}
