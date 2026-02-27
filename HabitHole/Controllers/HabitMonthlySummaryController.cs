using HabitHole.Models.Dto;
using HabitHole.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitHole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HabitMonthlySummaryController : ControllerBase
    {
        private readonly IHabitMonthlySummaryService _service;

        public HabitMonthlySummaryController(IHabitMonthlySummaryService service) => _service = service;

        [HttpGet("monthly-summary")]
        [ProducesResponseType(typeof(HabitMonthlySummaryDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlySummary(
           [FromQuery] string month,
           [FromQuery] bool includeInactive = false)
        {
            try
            {
                var result = await _service.GetMonthlySummaryAsync(
                    month,
                    includeInactive);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUpdatedStreak([FromQuery] int id)
        {
            var streak = await _service.GetUpdatedStreakAsync(id);
            return Ok(streak);
        }


        [HttpGet("monthly-consistency")]
        [ProducesResponseType(typeof(List<DailyConsistencyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMonthlyConsistency(int year, int month)
        {
            var result = await _service.GetMonthlyConsistency(year, month);
            return Ok(result);
        }

    }
}