using HabitHole.Services.Interfaces;
using System.Security.Claims;

namespace HabitHole.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId { get; }

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            UserId = accessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
