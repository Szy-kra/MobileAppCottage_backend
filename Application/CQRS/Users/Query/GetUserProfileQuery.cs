using MediatR;
using MobileAppCottage.Application.DTOs;

namespace MobileAppCottage.Application.CQRS.Users.Queries
{
    // To zapytanie mówi: "Chcę pobrać profil użytkownika i oczekuję w odpowiedzi UserProfileDto"
    public class GetUserProfileQuery : IRequest<UserProfileDto>
    {
        // To zapytanie nie potrzebuje parametrów (jak ID), 
        // bo ID użytkownika pobierzemy w Handlerze prosto z tokena przez IUserContext.
    }
}