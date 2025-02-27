using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace BussinesLogic.Services;

public class Service<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>(
    IRepository<T> repository,
    ILogger<Service<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>> logger,
    IMapper mapper)
    : IService<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto>
    where T : EntityBase
{
    public async Task<IEnumerable<TReadDto>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync");
        var entities = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<TReadDto>>(entities);

    }

    public async Task<TReadDetailDto> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync");
        var entity = await repository.GetByIdAsync(id);
        return mapper.Map<TReadDetailDto>(entity);
    }

    public async Task<TReadDto> AddAsync(TCreateDto dto)
    {
        logger.LogInformation("AddAsync");
        var entity = mapper.Map<T>(dto);
        entity = await repository.AddAsync(entity);
        return mapper.Map<TReadDto>(entity);
    }

    public async Task UpdateAsync(int id, TUpdateDto dto)
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

    public async Task DeleteAsync(int id)
    {
        logger.LogInformation("DeleteAsync");
        await repository.DeleteAsync(id);
    }
}