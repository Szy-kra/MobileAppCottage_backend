using MediatR;
using Microsoft.AspNetCore.Mvc;
using MobileAppCottage.Application.CQRS.Cottages.Commands;
using MobileAppCottage.Application.CQRS.Cottages.Queries;
using MobileAppCottage.Application.DTOs;

namespace MobileAppCottage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CottageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CottageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CottageDto>>> GetAll()
        {
            var dtos = await _mediator.Send(new GetAllCottagesQuery());
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CottageDto>> GetById([FromRoute] int id)
        {
            var dto = await _mediator.Send(new GetCottageByIdQuery(id));
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateCottageCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCottageCommand command)
        {
            // Przypisanie ID z trasy do komendy, aby handler wiedzia³, który obiekt edytowaæ
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Upewnij siê, ¿e DeleteCottageCommand przyjmuje ID w konstruktorze
            await _mediator.Send(new DeleteCottageCommand(id));
            return NoContent();
        }
    }
}