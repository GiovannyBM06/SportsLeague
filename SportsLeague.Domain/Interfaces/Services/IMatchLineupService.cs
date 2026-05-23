using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<IEnumerable<MatchLineup>> GetByMacthAsync(int MacthId);
        Task<IEnumerable<MatchLineup>> GetByMacthAndTeamAsync(int MacthId, int TeamId);
        Task<MatchLineup> CreateAsync(MatchLineup MatchLineup);
        Task DeleteAsync(int id);
    }
}