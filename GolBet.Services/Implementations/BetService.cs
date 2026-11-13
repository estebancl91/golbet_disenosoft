using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GolBet.Services.Implementations;

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BetService(
        IBetRepository betRepository,
        IMatchRepository matchRepository,
        UserManager<AppUser> userManager,
        AppDbContext context,
        IMapper mapper)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
    }

    public async Task PlaceBetAsync(string userId, BetFormDto dto)
    {
        // 1. Validar que el partido exista y acepte apuestas
        var match = await _matchRepository.GetByIdAsync(dto.MatchId);
        if (match is null || !match.IsActive)
            throw new InvalidOperationException("El partido no existe.");

        if (match.Status != MatchStatus.Scheduled || match.Date <= DateTime.UtcNow)
            throw new InvalidOperationException("Este partido ya no acepta apuestas.");

        // 2. Validar que no tenga la misma apuesta repetida
        if (await _betRepository.ExistsAsync(dto.MatchId, userId, dto.Pick))
            throw new InvalidOperationException("Ya tienes una apuesta con esta selección en este partido.");

        // 3. Buscar al usuario y validar saldo
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuario no encontrado.");

        if (dto.Amount > user.Balance)
            throw new InvalidOperationException($"Saldo insuficiente: tienes {user.Balance:N2} FutCoins.");

        // 4. Obtener las cuotas del partido según la selección
        decimal oddsAtPlacement = 0;

        if (dto.Pick == BetPick.Home)
        {
            oddsAtPlacement = match.HomeOdds;
        }
        else if (dto.Pick == BetPick.Draw || dto.Pick.ToString() == "Tie")
        {
            oddsAtPlacement = match.DrawOdds;
        }
        else if (dto.Pick == BetPick.Away)
        {
            oddsAtPlacement = match.AwayOdds;
        }
        else
        {
            throw new InvalidOperationException("Selección de apuesta inválida.");
        }

        // 5. Descontar el saldo y actualizar el usuario mediante UserManager
        user.Balance -= dto.Amount;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException("No se pudo actualizar el saldo del usuario.");
        }

        // 6. Crear y registrar la apuesta usando directamente el contexto compartido
        var bet = new Bet
        {
            MatchId = dto.MatchId,
            UserId = userId,
            Pick = dto.Pick,
            Amount = dto.Amount,
            OddsAtPlacement = oddsAtPlacement,
            Status = BetStatus.Pending
        };

        _context.Bets.Add(bet);

        // 7. Guardar todos los cambios de forma persistente y atómica en la base de datos
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BetDto>> GetMyBetsAsync(string userId)
        => _mapper.Map<IEnumerable<BetDto>>(await _betRepository.GetByUserAsync(userId));
}