using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface IMatchLineupRepository: IGenericRepository<MatchLineup>
    {
        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchid);
        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchid, int teamid);
        Task<bool> PlayerExistByMatchAndTeamAsync(int matchid, int teamid);
    }
}
