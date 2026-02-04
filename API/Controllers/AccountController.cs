using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Domain.Entities; // To załatwia błąd z klasą User [cite: 2026-02-04]
using System.Security.Claims; // To załatwia błąd z FindFirstValue [cite: 2026-02-04]

namespace MobileAppCottage.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        // UserManager musi być tutaj, aby Identity wiedziało, z której tabeli czytać [cite: 2026-02-04]
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Pobieramy ID z tokena Identity [cite: 2026-02-04]
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            // Szukamy usera w Identity EF [cite: 2026-02-04]
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            // Sprawdzamy rolę (Host/Guest) tak jak w Twoim pomyśle z AppWeb [cite: 2026-02-04]
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Guest";

            return Ok(new
            {
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                companyName = user.CompanyName,
                taxId = user.TaxId,
                role = userRole
            });
        }
    }
}