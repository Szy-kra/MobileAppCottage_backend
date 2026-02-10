using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.CQRS.Users.Queries;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.CQRS.Users.Handlers
{
    public class GetUserProfileQueryHandler(
        CottageDbContext dbContext,
        IUserContext userContext) : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser == null) return null!;

            var user = await dbContext.Users
                .Include(u => u.OwnedCottages)
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken);

            if (user == null) return null!;

            var now = DateTime.UtcNow;
            var response = new UserProfileDto
            {
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsHost = user.IsHost
            };

            if (user.IsHost)
            {
                response.MyCottages = user.OwnedCottages
                    .Select(c => new CottageDto { Id = c.Id, Name = c.Name })
                    .ToList();

                var hostCottageIds = user.OwnedCottages.Select(c => c.Id).ToList();
                var reservations = await dbContext.CottageReservations
                    .Where(r => hostCottageIds.Contains(r.CottageId))
                    .ToListAsync(cancellationToken);

                // NAPRAWA: Zmiana EndDate na To
                response.ActiveReservations = reservations.Where(r => r.To >= now).Select(r => new ReservationDto { Id = r.Id }).ToList();
                response.PastReservations = reservations.Where(r => r.To < now).Select(r => new ReservationDto { Id = r.Id }).ToList();
            }
            else
            {
                // GUEST: Filtrujemy po ReservedById zamiast nieistniejącego pola Email
                var reservations = await dbContext.CottageReservations
                    .Where(r => r.ReservedById == user.Id)
                    .ToListAsync(cancellationToken);

                response.ActiveReservations = reservations.Where(r => r.To >= now).Select(r => new ReservationDto { Id = r.Id }).ToList();
                response.PastReservations = reservations.Where(r => r.To < now).Select(r => new ReservationDto { Id = r.Id }).ToList();
            }

            return response;
        }
    }
}