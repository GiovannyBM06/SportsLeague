using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Net.Mail;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger
            )
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo Sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo Sponsor con ID {SponsorID}", id);
            var Sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (Sponsor == null)
            {
                _logger.LogInformation("No se encontró un Sponsor con ID {SponsorID}", id);
            }
            return Sponsor;
        }

        public async Task<Sponsor?> CreateAsync(Sponsor sponsor)
        {
            // validacion de email
            try
            {
                var Email = new MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException(
                $"El correo '{sponsor.ContactEmail}' no es un formato válido");
            }

            // Validación de nombre
            var sponsorExist = await _sponsorRepository.GetByNameAsync(sponsor.Name);
            if (sponsorExist != null)
            {
                _logger.LogWarning("Ya existe un Sponsor con nombre '{SponsorName}'", sponsor.Name);
                throw new InvalidOperationException(
                    $"Ya existe un equipo con el nombre '{sponsor.Name}'");
            }

            _logger.LogInformation("Creando Sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingSponsor == null)
            {
                throw new KeyNotFoundException($"No se encontró un Sponsor con ID {id}");
            }

            try
            {
                var Email = new MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException(
                $"El correo '{sponsor.ContactEmail}' no es un formato válido");
            }
            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;

            _logger.LogInformation("Actualizando Sponsor con ID {SponsorID}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingSponsor == null)
                throw new KeyNotFoundException($"No se encontró el Sponsor con ID {id}");

            _logger.LogInformation("Borrando Sponosor con ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task RegisterTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            if (contractAmount <= 0)
            {
                throw new InvalidOperationException($"El monto del contrato debe ser mayor a cero");
            }

            var existingSponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (existingSponsor == null)
            {
                throw new KeyNotFoundException($"No se encontró el Sponsor con ID {sponsorId}");
            }

            var existingTournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (existingTournament == null)
            {
                throw new KeyNotFoundException($"No se encontró el Torneo con ID {tournamentId}");
            }

            var alreadyRegistered = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (alreadyRegistered != null)
            {
                throw new InvalidOperationException($"El Sponsor con ID {sponsorId} ya está registrado en el Torneo con ID {tournamentId}");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                SponsorId = sponsorId,
                TournamentId = tournamentId,
                JoinedAt = DateTime.UtcNow,
                ContractAmount = contractAmount
            };

            _logger.LogInformation("Registrando Sponsor con ID {SponsorID} en el Torneo con ID {TournamentID}",
                sponsorId, tournamentId);
            await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (existingSponsor == null)
            {
                throw new KeyNotFoundException($"No se encontró el Sponsor con ID {sponsorId}");
            }

            var sponsoredTournaments = await _tournamentSponsorRepository.GetBySponsorAsync(sponsorId);
            return sponsoredTournaments
                    .Select(st => st.Tournament);
        }

        public async Task DeleteTournamentSponsorshipAsync(int sponsorId, int tournamentId)
        {
            var relationship = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId,sponsorId);
            if (relationship == null)
            {
                throw new KeyNotFoundException
                    ($"No se encontró un patrocinio entre el Sponsor con ID {sponsorId} y el Torneo con ID {tournamentId}");
            }
            await _tournamentSponsorRepository.DeleteAsync(relationship.Id);
            _logger.LogInformation(
                "Eliminando patrocinio entre Sponsor con ID {SponsorID} y Torneo con ID {TournamentID}", sponsorId, tournamentId);
        }
    }
}
