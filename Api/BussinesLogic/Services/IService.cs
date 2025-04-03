using Api.Infrastructure.Models;

namespace BussinesLogic.Services;

public interface IService<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto> where T : EntityBase
{
    Task<IEnumerable<TReadDto>> GetAllAsync();
    Task<TReadDetailDto> GetByIdAsync(Guid id);
    Task<TReadDto> AddAsync(TCreateDto dto);
    Task UpdateAsync(Guid id, TUpdateDto dto);
    Task DeleteAsync(Guid id);
}