using Microsoft.AspNetCore.Mvc;
using receivables.CrossCuting;
using receivables.Domain.DTOs.Cart;
using receivables.Domain.DTOs.Checkout;
using receivables.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace receivables.Application.Controllers
{
    [ApiController]
    [Route("v1/companies/{companyId}/cart")]
    [Produces("application/json")]
    [ExcludeFromCodeCoverage]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtém o carrinho de notas fiscais da empresa")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CartDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
        public async Task<ActionResult<CartDto>> GetCart([Required] Guid companyId)
        {
            var result = await _cartService.GetCartAsync(companyId);
            return Ok(result);
        }

        [HttpGet("checkout")]
        [SwaggerOperation(Summary = "Calcula o checkout do carrinho com valores líquidos e brutos")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CheckoutDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationErrorsDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
        public async Task<ActionResult<CheckoutDto>> GetCartCheckout([Required] Guid companyId)
        {
            var result = await _cartService.GetCartCheckoutAsync(companyId);
            return Ok(result);
        }

        [HttpPost("items")]
        [SwaggerOperation(Summary = "Adiciona uma nota fiscal ao carrinho")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CartDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationErrorsDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
        public async Task<ActionResult<CartDto>> AddInvoiceToCart(
            [Required] Guid companyId,
            [FromBody] CartItemRequest request)
        {
            var result = await _cartService.AddInvoiceToCartAsync(companyId, request.InvoiceId);
            return Ok(result);
        }

        [HttpDelete("items/{invoiceId}")]
        [SwaggerOperation(Summary = "Remove uma nota fiscal do carrinho")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CartDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationErrorsDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
        public async Task<ActionResult<CartDto>> RemoveInvoiceFromCart(
            [Required] Guid companyId,
            [Required] Guid invoiceId)
        {
            var result = await _cartService.RemoveInvoiceFromCartAsync(companyId, invoiceId);
            return Ok(result);
        }
    }
}
