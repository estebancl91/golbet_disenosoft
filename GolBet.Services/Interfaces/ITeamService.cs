// GolBet.Services/Interfaces/ITeamService.cs
using AutoMapper;
using GolBet.Entities;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;

namespace GolBet.Services.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllAsync();
    Task<TeamFormDto?> GetForEditAsync(int id);
    Task CreateAsync(TeamFormDto dto);
    Task UpdateAsync(TeamFormDto dto);
    Task DeactivateAsync(int id);
}

