using GolBet.Entities.Enums;
using GolBet.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GolBet.Services.Interfaces
{
    public interface IMatchService
    {
        // Métodos que ya tenías
        Task<IEnumerable<MatchDto>> GetAllWithTeamsAsync(MatchStatus? status = null, string? search = null);
        Task<MatchDto> GetByIdAsync(int id);

        // Métodos NUEVOS que pide el controlador
        Task<MatchDto> GetByIdWithTeamsAsync(int id);
        Task UpdateAsync(MatchDto matchDto);
    }
}