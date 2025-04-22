using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Produces("application/json")]
[Authorize]
[ApiController]
public class BaseApiController<T, TReadDto, TDetailDto, TCreateDto, TUpdateDto> : ControllerBase
    where T : EntityBase
{

    protected Guid UserId => Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!.Value);

    private readonly IService<T, TReadDto, TDetailDto, TCreateDto, TUpdateDto> _service;

    public BaseApiController(IService<T, TReadDto, TDetailDto, TCreateDto, TUpdateDto> service)
    {
        _service = service;
    }

    /// <summary>
    ///     Retrieves all entities.
    /// </summary>
    /// <returns>List of entities</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<PaginationResponse<TReadDto>>> GetAll([FromQuery] PaginationRequest request)
    {
        return Ok(await _service.GetAllAsync(request));
    }

    /// <summary>
    ///     Retrieves an entity by ID.
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<TDetailDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    ///     Creates a new entity.
    /// </summary>
    /// <param name="dto">Entity data</param>
    /// <returns>Created entity</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async ValueTask<ActionResult<TReadDto>> Create([FromBody] TCreateDto dto)
    {
        var createdDto = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdDto }, createdDto);
    }

    /// <summary>
    ///     Updates an existing entity.
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="dto">Updated entity data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<NoContentResult> Update(Guid id, [FromBody] TUpdateDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    ///     Deletes an entity by ID.
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}