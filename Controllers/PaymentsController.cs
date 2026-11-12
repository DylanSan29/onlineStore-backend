using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(PaymentService payments) : ControllerBase
{
    [HttpPost("orders/{orderId:int}/checkout")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout(int orderId)
    {
        try
        {
            var result = await payments.CreateCheckoutAsync(CurrentUserId(), orderId);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    [HttpGet("orders/{orderId:int}/status")]
    [Authorize]
    public async Task<IActionResult> GetStatus(int orderId)
    {
        var result = await payments.GetStatusAsync(CurrentUserId(), orderId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("webhooks/mercadopago")]
    [AllowAnonymous]
    public IActionResult MercadoPagoWebhook(PaymentWebhookRequest request)
    {
        return Accepted(new { received = true, processed = false, message = "Webhook verification and processing are pending provider integration." });
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
