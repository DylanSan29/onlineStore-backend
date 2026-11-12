using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/users/me/addresses")]
public sealed class AddressesController(AddressService addresses) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await addresses.GetForUserAsync(CurrentUserId()));

    [HttpPost]
    public async Task<IActionResult> Create(AddressRequest request) => Ok(await addresses.CreateAsync(CurrentUserId(), request));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AddressRequest request) => await addresses.UpdateAsync(CurrentUserId(), id, request) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => await addresses.DeleteAsync(CurrentUserId(), id) ? NoContent() : NotFound();

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
