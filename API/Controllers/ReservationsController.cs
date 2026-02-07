using MediatR;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Application.Reservations.Commands;
using MobileAppCottage.Application.Reservations.Queries;

namespace MobileAppCottage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Używamy spójnego nazewnictwa z CottageController
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Pobiera listę rezerwacji dla konkretnego domku (pod kalendarz w React Native).
        /// </summary>
        [HttpGet("cottage/{cottageId}")]
        public async Task<ActionResult<List<ReservationDto>>> GetByCottage(int cottageId)
        {
            var result = await _mediator.Send(new GetCottageReservationsQuery(cottageId));
            return Ok(result);
        }

        /// <summary>
        /// Tworzy nową rezerwację.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateReservationCommand command)
        {
            // Zwraca ID nowej rezerwacji, podobnie jak CreateCottageCommand
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        /// <summary>
        /// Usuwa rezerwację po ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Podobna logika jak w DeleteCottageCommand
            await _mediator.Send(new DeleteReservationCommand(id));
            return NoContent();
        }
    }
}