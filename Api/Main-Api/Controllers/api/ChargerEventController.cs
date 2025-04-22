using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Domain;
using GeneratedDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Produces("application/json")]
[Authorize]
[Route("api/charger-events")]
[ApiController]
public class ChargerEventController(
    IService<ChargerEvent, ChargerEventReadDto, ChargerEventReadDetailDto, ChargerEventCreateDto, ChargerEventUpdateDto> service,
    ILogger<ChargerEventController> logger) : ControllerBase
{
    private readonly ILogger<ChargerEventController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<PaginationResponse<ChargerEventReadDto>>> GetAll([FromQuery] PaginationRequest request)
    {
        return Ok(await service.GetAllAsync(request));
    }
}