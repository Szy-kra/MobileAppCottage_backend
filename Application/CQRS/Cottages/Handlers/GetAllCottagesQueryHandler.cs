using AutoMapper;
using MediatR;
using MobileAppCottage.Application.CQRS.Cottages.Queries;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Domain.Interfaces;

namespace MobileAppCottage.Application.CQRS.Cottages.Handlers
{
    public class GetAllCottagesQueryHandler : IRequestHandler<GetAllCottagesQuery, IEnumerable<CottageDto>>
    {
        private readonly ICottageRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCottagesQueryHandler(ICottageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CottageDto>> Handle(GetAllCottagesQuery request, CancellationToken cancellationToken)
        {
            var cottages = await _repository.GetAll();
            return _mapper.Map<IEnumerable<CottageDto>>(cottages);
        }
    }
}