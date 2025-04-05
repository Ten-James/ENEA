using Api.Infrastructure.Models;
using Domain;

namespace BussinesLogic.Services;

public interface IService<T, TReadDto, TReadDetailDto, TCreateDto, TUpdateDto> where T : EntityBase
{
    Task<PaginationResponse<TReadDto>> GetAllAsync(PaginationRequest paginationRequest);
    Task<TReadDetailDto> GetByIdAsync(Guid id);
    Task<TReadDto> AddAsync(TCreateDto dto);
    Task UpdateAsync(Guid id, TUpdateDto dto);
    Task DeleteAsync(Guid id);
}