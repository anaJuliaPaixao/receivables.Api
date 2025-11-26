using Microsoft.AspNetCore.Mvc;
using receivables.CrossCuting;
using receivables.Domain.DTOs.Invoice;
using receivables.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace receivables.Application.Controllers;

[ApiController]
[Route("v1")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
[ApiExplorerSettings(GroupName = "v1")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoicesService _invoicesService;

    public InvoicesController(IInvoicesService invoicesService)
    {
        _invoicesService = invoicesService;
    }

    [HttpPost]
    [Route("invoices")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvoiceDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorsDTO))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionDTO))]
    public async Task<ActionResult<InvoiceDto>> CreateInvoices(InvoiceRequest invoiceRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationErrorsDTO(ModelState));

        try
        {
            var result = await _invoicesService.CreateAsync(invoiceRequest);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ExceptionDTO { ErrorMessage = ex.Message });
        }
    }

    [HttpPut]
    [Route("invoices/{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(InvoiceDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorsDTO))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionDTO))]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(Guid id, UpdateInvoiceRequest updateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationErrorsDTO(ModelState));

        try
        {
            var result = await _invoicesService.UpdateAsync(id, updateRequest);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ExceptionDTO { ErrorMessage = ex.Message });
        }
    }
}