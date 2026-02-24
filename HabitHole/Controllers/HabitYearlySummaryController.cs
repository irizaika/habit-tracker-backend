using HabitHole.Models.Dto;
using HabitHole.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitHole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HabitYearlySummaryController : ControllerBase
    {
        private readonly IHabitYearlySummaryService _service;

        public HabitYearlySummaryController(IHabitYearlySummaryService service)
        {
            _service = service;
        }

        [HttpGet("yearly-summary")]
        [ProducesResponseType(typeof(List<HabitDailySummaryDto>), StatusCodes.Status200OK)]
        public async Task<List<HabitDailySummaryDto>> GetYearlyCalendar(int year)
        {
            var entries = await _service.GetYearlyCalendar(year);
            return entries;
        }

        [HttpGet("yearly-habit-summary")]
        [ProducesResponseType(typeof(List<HabitYearlySummaryDto>), StatusCodes.Status200OK)]
        public async Task<List<HabitYearlySummaryDto>> GetYearlyHabitCalendar(int year)
        {
            var result = await _service.GetYearlyHabitCalendar(year);
            return result;
        }
    }
}