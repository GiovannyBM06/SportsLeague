using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using System.Text.RegularExpressions;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
    {
        public MatchLineupRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchid)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                .ThenInclude(p => p.Team)
                .Where(ml => ml.MatchId == matchid)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchid, int teamid)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                .ThenInclude(p => p.Team)
                .Where(ml => ml.MatchId == matchid && ml.Player.TeamId == teamid)
                .ToListAsync();
        }

        public async Task<bool> PlayerExistByMatchAndTeamAsync(int matchid, int teamid)
        {
            return await _dbSet
                .AnyAsync(ml => ml.MatchId == matchid && ml.Player.TeamId == teamid);
        }
    }
}
