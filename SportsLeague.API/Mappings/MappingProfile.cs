using AutoMapper;
using SportsLeague.API.DTOs.Enums;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;

namespace SportsLeague.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Team
            CreateMap<TeamRequestDTO, Team>();
            CreateMap<Team, TeamResponseDTO>();
            // Player
            CreateMap<PlayerRequestDTO, Player>();
            CreateMap<Player, PlayerResponseDTO>()
                .ForMember(
                    dest=> dest.TeamName,
                    opt=> opt.MapFrom(src=>src.Team.Name));
        }
    }
}