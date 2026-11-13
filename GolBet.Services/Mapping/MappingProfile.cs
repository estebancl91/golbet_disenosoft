using AutoMapper;
using GolBet.Entities;
using GolBet.Services.DTOs;

namespace GolBet.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Flattening by convention:
        // MatchDto.HomeTeamName  <- Match.HomeTeam.Name
        // MatchDto.AwayTeamCrestUrl <- Match.AwayTeam.CrestUrl
        CreateMap<Match, MatchDto>();
        CreateMap<Team, TeamDto>();
        CreateMap<TeamFormDto, Team>().ReverseMap();
        CreateMap<MatchFormDto, Match>().ReverseMap();

        CreateMap<Bet, BetDto>()
            .ForMember(dto => dto.PotentialPayout,
                       options => options.MapFrom(bet => bet.Amount * bet.OddsAtPlacement));

        // ¡Esta es la regla que faltaba para solucionar el error!
        CreateMap<MatchDto, MatchDetailDto>();
    }
}