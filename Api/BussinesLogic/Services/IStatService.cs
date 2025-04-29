using Domain;

namespace BusinessLogic.Interfaces;

public interface IStatService
{
    Task<Dictionary<string, MonthStatDto>> GetAllStatsAsync();

    Task<Dictionary<string, MonthStatDto>> GetAllStatsAsync(Guid? userId);
}