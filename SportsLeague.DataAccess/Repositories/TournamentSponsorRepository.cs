using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository: GenericRepository<TournamentSponsor> , ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {
        }
        // obtener la relación entre un torneo y un patrocinador específico
        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId)
                .FirstOrDefaultAsync();
        }
        // obtener todos los patrocinadores asociados a un torneo específico
        public async Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(int tournamentId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId)
                .Include(ts=>ts.Sponsor)
                .ToListAsync();
        }
        //obtener todos los torneos asociados a un patrocinador específico
        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync (int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .ToListAsync();
        }
    }
}
