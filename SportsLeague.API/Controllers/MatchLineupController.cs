using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(IMatchLineupService matchLineupService, IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        [HttpPost("lineup")]
        public async Task<ActionResult<MatchLineupResponseDTO>> CreateMatchLineup(
            int matchId,
            MatchLineupRequestDTO dto)
        {
            try
            {
                var lineup = _mapper.Map<MatchLineup>(dto);
                lineup.MatchId = matchId;

                var createdLineup = await _matchLineupService.CreateAsync(lineup);
                var responseDto = _mapper.Map<MatchLineupResponseDTO>(createdLineup);

                return CreatedAtAction(
                    nameof(GetMatchLineup),
                    new { matchId = matchId },
                    responseDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("lineup")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetMatchLineup(int matchId)
        {
            try
            {
                var lineups = await _matchLineupService.GetByMacthAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("lineup/team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetMatchLineupByTeam(
            int matchId,
            int teamId)
        {
            try
            {
                var lineups = await _matchLineupService.GetByMacthAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("lineup/{id}")]
        public async Task<ActionResult> DeleteMatchLineup(int matchId, int id)
        {
            try
            {
                await _matchLineupService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
