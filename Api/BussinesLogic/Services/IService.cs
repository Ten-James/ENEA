using Api.Infrastructure.Models;

namespace BussinesLogic.Services;

public interface IService<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto> where T : EntityBase
{
    Task<IEnumerable<TReadDto>> GetAllAsync();
    Task<TReadDetailDto> GetByIdAsync(int id);
    Task<TReadDto> AddAsync(TCreateDto dto);
    Task UpdateAsync(int id, TUpdateDto dto);
    Task DeleteAsync(int id);
}