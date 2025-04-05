using Api.BussinesLogic.Exceptions;
using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Logging;

namespace BussinesLogic.Services;

public class Service<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>(
    IRepository<T> repository,
    ILogger<Service<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>> logger,
    IMapper mapper)
    : IService<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>
    where T : EntityBase
{
    public async Task<PaginationResponse<TReadDto>> GetAllAsync(PaginationRequest request)
    {
        logger.LogInformation("GetAllAsync");
        var pageNumber = request.PageNumber;
        if (pageNumber < 0)
        {
            pageNumber = 0;
        }
        var totalCount = await repository.CountAsync();
        var offset = pageNumber * request.PageSize;
        while (offset > totalCount)
        {
            offset -= request.PageSize;
            pageNumber--;
        }

        var entities = await repository.GetAllAsync(offset, request.PageSize);
        var dtos = mapper.Map<List<TReadDto>>(entities);
        var response = new PaginationResponse<TReadDto>(totalCount, pageNumber, request.PageSize, dtos);
        return response;
    }

    public async Task<TReadDetailDto> GetByIdAsync(Guid id)
    {
        logger.LogInformation("GetByIdAsync");
        var entity = await repository.GetByIdAsync(id);

        if (entity == null)
        {
            throw new EntityNotFoundException(id, "Entity not found");
        }

        return mapper.Map<TReadDetailDto>(entity);
    }

    public async Task<TReadDto> AddAsync(TCreateDto dto)
    {
        logger.LogInformation("AddAsync");
        var entity = mapper.Map<T>(dto);
        entity = await repository.AddAsync(entity);
        return mapper.Map<TReadDto>(entity);
    }

    public async Task UpdateAsync(Guid id, TUpdateDto dto)
    {
        logger.LogInformation("UpdateAsync");
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception("Entity not found");
        }
        var updatedEntity = mapper.Map(dto, entity);
        await repository.UpdateAsync(updatedEntity);

    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("DeleteAsync");
        await repository.DeleteAsync(id);
    }
}