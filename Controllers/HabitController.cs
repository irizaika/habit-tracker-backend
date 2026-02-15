using HabitHole.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace HabitHole.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    [Route("api/habits")]
    public class HabitController : ControllerBase
    {       
        private readonly IHabitService _habitService;

        public HabitController(IHabitService habitService)
        {
            _habitService = habitService;
        }

        // GET api/habits
        [HttpGet]
        [ProducesResponseType(typeof(HabitDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHabits(GetHabitsDto dto)
        {
            var habits = await _habitService.GetHabitsAsync(dto.Year, dto.Month);
            return Ok(habits);
        }

        // POST api/habits
        [HttpPost]
        [ProducesResponseType(typeof(HabitDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateHabit(CreateHabitDto dto)
        {
            var habit = await _habitService.CreateHabitAsync(dto.Name, dto.GoalCount, dto.validFrom, dto.validTo);
            return CreatedAtAction(nameof(GetHabits), new { id = habit.Id }, habit);
        }

        // PUT api/habits/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateHabit(int id, UpdateHabitDto dto)
        {
            await _habitService.UpdateHabitAsync(id, dto.Name, dto.GoalCount, dto.validFrom, dto.validTo);
            return NoContent();
        }

        // DELETE api/habits/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteHabit(int id)
        {
            await _habitService.DeleteHabitAsync(id);
            return NoContent();
        }
    }

    public record GetHabitsDto(int Month, int Year); 
    public record CreateHabitDto(string Name, int GoalCount, DateOnly validFrom, DateOnly? validTo); 
    public record UpdateHabitDto(string Name, int GoalCount, DateOnly validFrom, DateOnly? validTo);
}
