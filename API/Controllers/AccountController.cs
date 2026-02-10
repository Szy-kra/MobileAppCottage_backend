using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Application.CQRS.Users.Handlers;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;

namespace MobileAppCottage.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    // Używamy Primary Constructor (.NET 8) z nazwą userContext
    public class AccountController(
        UserManager<User> userManager,
        IUserContext userContext,
        IMediator mediator) : ControllerBase
    {
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Korzystamy z ujednoliconej nazwy userContext
            var currentUser = userContext.GetCurrentUser();
            if (currentUser == null) return Unauthorized();

            var user = await userManager.FindByIdAsync(currentUser.Id);
            if (user == null) return NotFound();

            var roles = await userManager.GetRolesAsync(user);

            return Ok(new
            {
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                companyName = user.CompanyName,
                taxId = user.TaxId,
                role = roles.FirstOrDefault() ?? "User"
            });
        }

        [HttpDelete("delete-account")]
        public async Task<ActionResult> DeleteAccount()
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser == null) return Unauthorized();

            // Wysyłamy komendę do handlera - kontroler widzi DeleteUserCommand dzięki poprawionemu usingowi
            await mediator.Send(new DeleteUserCommand(currentUser.Id));

            return NoContent();
        }
    }
}