using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GolBet.Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public MatchService(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> GetAllWithTeamsAsync(MatchStatus? status = null, string? search = null)
        {
            var matches = await _matchRepository.GetAllWithTeamsAsync(status, search);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDto> GetByIdAsync(int id)
        {
            // Usa el método base del repositorio genérico
            var match = await _matchRepository.GetByIdAsync(id);
            return _mapper.Map<MatchDto>(match);
        }

        public async Task<MatchDto> GetByIdWithTeamsAsync(int id)
        {
            // Usa tu nuevo método específico que incluye .Include()
            var match = await _matchRepository.GetByIdWithTeamsAsync(id);
            return _mapper.Map<MatchDto>(match);
        }

        public async Task UpdateAsync(MatchDto matchDto)
        {
            var match = _mapper.Map<Match>(matchDto);

            // Si tu IGenericRepository tiene un método UpdateAsync, esto funcionará perfecto.
            // Si te marca error aquí, cámbialo simplemente por: _matchRepository.Update(match);
            await _matchRepository.UpdateAsync(match);
        }
    }
}