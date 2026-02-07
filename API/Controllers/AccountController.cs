using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces; // Musi być do IUserContext
using System.Security.Claims;

namespace MobileAppCottage.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserContext _userContext; // Dodajemy nasz kontekst

        public AccountController(UserManager<User> userManager, IUserContext userContext)
        {
            _userManager = userManager;
            _userContext = userContext;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

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

        // --- TO JEST TA BRAKUJĄCA METODA USUWANIA ---
        [HttpDelete("delete-account")]
        public async Task<ActionResult> DeleteAccount()
        {
            // 1. Pobieramy dane zalogowanego usera z naszego nowego serwisu
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
                return Unauthorized();

            // 2. Szukamy go w bazie Identity
            var user = await _userManager.FindByIdAsync(currentUser.Id);

            if (user == null)
                return NotFound();

            // 3. Usuwamy go fizycznie z bazy danych
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest("Błąd podczas usuwania konta.");

            return NoContent(); // Zwraca 204 - sukces, konto skasowane
        }
    }
}