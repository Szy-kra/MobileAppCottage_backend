using AutoMapper;
using MobileAppCottage.Application.CQRS.Cottages.Commands;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Domain.Entities;

namespace MobileAppCottage.Application.Mappings
{
    public class CottageMappingProfile : Profile
    {
        public CottageMappingProfile()
        {
            // 1. KIERUNEK: Z bazy do aplikacji (Entity -> DTO)
            CreateMap<Cottage, CottageDto>()
                .ForMember(d => d.City, o => o.MapFrom(s => s.ContactDetails.City))
                .ForMember(d => d.Street, o => o.MapFrom(s => s.ContactDetails.Street))
                .ForMember(d => d.PostalCode, o => o.MapFrom(s => s.ContactDetails.PostalCode))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.ContactDetails.Price))
                .ForMember(d => d.MaxPersons, o => o.MapFrom(s => s.ContactDetails.MaxPersons))
                // MAPOWANIE OPISÓW:
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description)) // Krótki opis (zajawka)
                .ForMember(d => d.About, o => o.MapFrom(s => s.About))             // Długi opis (pełny)
                .ForMember(d => d.EncodedName, o => o.MapFrom(s => s.EncodedName))
                .ForMember(d => d.ImageUrls, o => o.MapFrom(s => s.Images.Select(i => i.Url)))
                .ForMember(d => d.BookedDates, o => o.MapFrom(s => s.Reservations));

            // 2. KIERUNEK: Z żądania do bazy (Command -> Entity)
            CreateMap<UpdateCottageCommand, Cottage>()
                .ForMember(dest => dest.ContactDetails, opt => opt.MapFrom(src => new CottageDetails
                {
                    City = src.City,
                    Street = src.Street,
                    PostalCode = src.PostalCode,
                    Price = src.Price,
                    MaxPersons = src.MaxPersons,
                    Description = null // Pole techniczne w ContactDetails zostawiamy puste
                }))
                // MAPOWANIE PÓL GŁÓWNYCH:
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) // Z komendy do bazy
                .ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About))             // Z komendy do bazy
                .ForPath(dest => dest.Id, opt => opt.Ignore());
        }
    }
}