using Microsoft.AspNetCore.Mvc;
using receivables.CrossCuting;
using receivables.Domain.DTOs.Checkout;
using receivables.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace receivables.Application.Controllers;

[ApiController]
[Route("v1/invoices")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
[ApiExplorerSettings(GroupName = "v1")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpGet("{invoiceId}/calculate")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(InvoiceCalculationDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationErrorsDTO))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
    public async Task<ActionResult<InvoiceCalculationDto>> CalculateInvoice([Required] Guid invoiceId)
    {
        var result = await _checkoutService.CalculateCheckoutByInvoiceIdAsync(invoiceId);
        return Ok(result);
    }
}
