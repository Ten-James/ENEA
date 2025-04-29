using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories;
using BusinessLogic.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class StatService : IStatService
{
    private readonly IRepository<ChargerEvent> _repository;

    public StatService(IRepository<ChargerEvent> repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<string, MonthStatDto>> GetAllStatsAsync()
    {
        var events = await _repository.GetAll().ToListAsync();
        return events
            .GroupBy(e => e.StartTime.ToString("yyyy-MM"))
            .ToDictionary(
                keySelector: g => g.Key,
                elementSelector: g => new MonthStatDto { TotalCharged = g.Sum(x => x.EnergyConsumed ?? 0), TotalCost = g.Sum(x => x.TotalPrice ?? 0) }
            );
    }

    public async Task<Dictionary<string, MonthStatDto>> GetAllStatsAsync(Guid? userId)
    {
        var events = await _repository.GetAll()
            .Where(e => e.UserId == userId)
            .ToListAsync();
        return events
            .GroupBy(e => e.StartTime.ToString("yyyy-MM"))
            .ToDictionary(
                keySelector: g => g.Key,
                elementSelector: g => new MonthStatDto { TotalCharged = g.Sum(x => x.EnergyConsumed ?? 0), TotalCost = g.Sum(x => x.TotalPrice ?? 0) }
            );
    }
}