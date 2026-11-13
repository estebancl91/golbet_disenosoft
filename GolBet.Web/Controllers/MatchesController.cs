using AutoMapper;
using GolBet.Entities.Enums;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GolBet.Web.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public MatchesController(IMatchService matchService, IMapper mapper)
        {
            _matchService = matchService;
            _mapper = mapper;
        }

        // GET: /Matches
        public async Task<IActionResult> Index(MatchStatus? status, string search)
        {
            var matches = await _matchService.GetAllWithTeamsAsync(status, search);

            // Compartir con la vista para mantener la persistencia visual
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;

            return View(matches);
        }

        // GET: /Matches/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // AHORA SÍ: Usamos el método que trae los equipos para que aparezcan junto al "VS"
            var match = await _matchService.GetByIdWithTeamsAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            // Mapeo al DTO que exige la vista
            var matchDetail = _mapper.Map<MatchDetailDto>(match);

            return View(matchDetail);
        }

        // GET: /Matches/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match == null) return NotFound();

            // Si tu vista Edit.cshtml exige explícitamente un MatchFormDto, descomenta estas dos líneas y borra el return View(match):
            // var matchForm = _mapper.Map<MatchFormDto>(match);
            // return View(matchForm);

            return View(match);
        }

        // POST: /Matches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MatchDto matchDto)
        {
            if (ModelState.IsValid)
            {
                // Guarda los cambios de la nueva cuota
                await _matchService.UpdateAsync(matchDto);
                return RedirectToAction(nameof(Index));
            }
            return View(matchDto);
        }
    }
}