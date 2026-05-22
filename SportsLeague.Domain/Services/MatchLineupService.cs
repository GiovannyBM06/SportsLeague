using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _lineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly MatchValidationHelper _validationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository lineupRepository,
            IMatchRepository matchRepository,
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            MatchValidationHelper validationHelper,
            ILogger<MatchLineupService> logger)
        {
            _lineupRepository = lineupRepository;
            _matchRepository = matchRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _validationHelper = validationHelper;
            _logger = logger;
        }

        public async Task<IEnumerable<MatchLineup>> GetByMacthAsync(int MacthId)
        {

            var match = await _matchRepository.ExistsAsync(MacthId);
            if (!match)
                throw new KeyNotFoundException(
                    $"No se encontró el partido con Id {MacthId}");

            _logger.LogInformation("Obteniendo la alineacion del partido {MatchId}", MacthId);
            return await _lineupRepository.GetByMatchAsync(MacthId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMacthAndTeamAsync(int MacthId, int TeamId)
        {

            var match = await _matchRepository.ExistsAsync(MacthId);
            if (!match)
                throw new KeyNotFoundException(
                    $"No se encontró el partido con Id {MacthId}");

            var team = await _teamRepository.ExistsAsync(TeamId);
            if (!team)
                throw new KeyNotFoundException(
                    $"No se encontró el equipo con Id {TeamId}");

            _logger.LogInformation("Obteniendo la alineacion del equipo {TeamId} en el partido {MacthId}", TeamId, MacthId);
            return await _lineupRepository.GetByMatchAndTeamAsync(MacthId, TeamId);
        }

        public async Task<MatchLineup> CreateAsync(MatchLineup MatchLineup)
        {
            // El partido debe existir
            var match = await _matchRepository.GetByIdAsync(MatchLineup.MatchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {MatchLineup.MatchId}");

            // Estado del partido 
            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

            // Jugador debe existir y pertenecer a un equipo que juega el partido
            await _validationHelper.ValidatePlayerInMatchAsync(MatchLineup.PlayerId, match);

            // Validar que el jugador no esté registrado en la alineación del partido
            var alreadyRegistered = await _lineupRepository.PlayerExistByMatchAndTeamAsync(MatchLineup.MatchId, MatchLineup.PlayerId);
            if (alreadyRegistered)
                throw new InvalidOperationException(
                    "El jugador ya está registrado en la alineación de este partido");

            // Solo pueden haber 11 titulares por equipo
            if (MatchLineup.IsStarter)
            {
                var Lineup = await _lineupRepository.GetByMatchAndTeamAsync(MatchLineup.MatchId, MatchLineup.Player.TeamId);
                int startersCount = Lineup.Count(ml => ml.IsStarter);
                if (startersCount == 11)
                    throw new InvalidOperationException($"El equipo ya tiene 11 titulares");
            }

            _logger.LogInformation(
                "Adding player {PlayerId} to lineup of match {MatchId}");

            return await _lineupRepository.CreateAsync(MatchLineup);
        }

        public async Task DeleteAsync(int id)
        {
            var lineup = await _lineupRepository.GetByIdAsync(id);
            if (lineup == null)
                throw new KeyNotFoundException($"No se encontró la alineación con ID {id}");

            // El partido debe estar en estado Scheduled
            var match = await _matchRepository.GetByIdAsync(lineup.MatchId);
            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden eliminar alineaciones de partidos Scheduled");

            _logger.LogInformation("Eliminando alineación con ID {LineupId} del partido {MatchId}", id, lineup.MatchId);
            await _lineupRepository.DeleteAsync(id);
        }
    }
}
