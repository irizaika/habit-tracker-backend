using HabitHole.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitHole.Controllers
{
    [ApiController]
    [Route("api/habits/{habitId}/entries")]
    [Authorize]
    public class HabitEntriesController : ControllerBase
    {
        private readonly IHabitEntryService _service;

        public HabitEntriesController(IHabitEntryService service)
        {
            _service = service;
        }

        // GET api/habits/{habitId}/entries?month=2026-02
        [HttpGet]
        public async Task<IActionResult> GetEntries(int habitId, [FromQuery] string month)
        {
            try
            {
                var result = await _service.GetEntriesAsync(habitId, month);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/habits/{habitId}/entries
        [HttpPost]
        public async Task<IActionResult> AddEntry(int habitId, HabitEntryDto dto)
        {
            try
            {
                await _service.AddEntryAsync(habitId, dto.Date);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE api/habits/{habitId}/entries?date=2026-02-04
        [HttpDelete]
        public async Task<IActionResult> RemoveEntry(int habitId, HabitEntryDto dto)
        {
            try
            {
                await _service.RemoveEntryAsync(habitId, dto.Date);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    public record HabitEntryDto(DateOnly Date);
}
