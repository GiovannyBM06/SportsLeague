using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;


namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
        Task RegisterTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount);
        Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId);
        Task DeleteTournamentSponsorshipAsync(int sponsorId, int tournamentId);
        Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category);
    }
}
