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
            var result = await payments.CreateCheckoutAsync(CurrentUserId(), orderId, Request.Headers["X-Idempotency-Key"].FirstOrDefault());
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
    public async Task<IActionResult> MercadoPagoWebhook(PaymentWebhookRequest request, [FromHeader(Name = "X-Webhook-Signature")] string? signature)
    {
        var processed = await payments.ProcessWebhookAsync(request, signature);
        return processed ? Ok(new { received = true, processed = true }) : Unauthorized(new { message = "Invalid webhook signature or payment." });
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
