using Microsoft.AspNetCore.Mvc;
using receivables.CrossCuting;
using receivables.Domain.DTOs.Company;
using receivables.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace receivables.Application.Controllers;

[ApiController]
[Route("v1")]
[Produces("application/json")]
[ExcludeFromCodeCoverage]
[ApiExplorerSettings(GroupName = "v1")]
public class CompaniesController : ControllerBase
{

    private readonly ICompaniesService _companiesService;

    public CompaniesController(ICompaniesService companiesService)
    {
        _companiesService = companiesService;
    }

    [HttpPost]
    [Route("companies")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CompanyDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationErrorsDTO))]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ExceptionDTO))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionDTO))]
    public async Task<ActionResult<CompanyDto>> CreateCompanie(CompanyRequest companyRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationErrorsDTO(ModelState));

        var result = await _companiesService.CreateAsync(companyRequest);
        return Ok(result);

    }

}
