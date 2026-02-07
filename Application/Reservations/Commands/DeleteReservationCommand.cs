using MediatR;

namespace MobileAppCottage.Application.Reservations.Commands
{
    /// <summary>
    /// Komenda do usuwania rezerwacji. 
    /// IRequest oznacza, że nie zwracamy wyniku (void).
    /// </summary>
    public class DeleteReservationCommand : IRequest
    {
        public int Id { get; }

        public DeleteReservationCommand(int id)
        {
            Id = id;
        }
    }
}