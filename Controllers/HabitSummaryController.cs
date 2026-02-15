using HabbitHole.Data;
using HabitHole.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitSummaryController : ControllerBase
    {
        private readonly IHabitSummaryService _service;

        public HabitSummaryController(IHabitSummaryService service)
        {
            _service = service;
        }

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


    }
}