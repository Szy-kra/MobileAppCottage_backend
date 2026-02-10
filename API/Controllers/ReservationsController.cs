using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Application.CQRS.Reservations.Commands;
using MobileAppCottage.Application.DTOs;

namespace MobileAppCottage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Wykorzystanie Primary Constructor - standard w .NET 8 [cite: 2026-01-12]
    public class ReservationsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Pobiera listę rezerwacji dla konkretnego domku.
        /// </summary>
        [HttpGet("cottage/{cottageId}")]
        public async Task<ActionResult<List<ReservationDto>>> GetByCottage(int cottageId)
        {
            // Metoda asynchroniczna musi mieć await. 
            // Jeśli nie masz jeszcze Query, możesz użyć Task.FromResult, ale docelowo wyślij zapytanie do mediatora.
            var reservations = await Task.FromResult(new List<ReservationDto>());
            return Ok(reservations);
        }

        /// <summary>
        /// Tworzy nową rezerwację.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create([FromBody] CreateReservationCommand command)
        {
            var id = await mediator.Send(command);
            return Ok(id);
        }

        /// <summary>
        /// Aktualizacja rezerwacji przez Hosta.
        /// </summary>
        [HttpPut("{id}/host-management")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> HostUpdate([FromRoute] int id, [FromBody] UpdateReservationByHostCommand command)
        {
            command.Id = id;
            await mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Usuwa rezerwację.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            // NAPRAWA CS1998: Dodanie 'await' przed wywołaniem mediatora.
            // To sprawia, że wątek jest zwalniany do puli podczas operacji na bazie danych.
            await mediator.Send(new DeleteReservationCommand(id));

            return NoContent();
        }
    }
}