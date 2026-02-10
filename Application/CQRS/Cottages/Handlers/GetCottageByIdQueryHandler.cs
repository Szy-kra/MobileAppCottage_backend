using AutoMapper;
using MediatR;
using MobileAppCottage.Application.CQRS.Cottages.Queries;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Domain.Exceptions;
using MobileAppCottage.Domain.Interfaces;

namespace MobileAppCottage.Application.CQRS.Cottages.Handlers
{
    public class GetCottageByIdQueryHandler : IRequestHandler<GetCottageByIdQuery, CottageDto>
    {
        private readonly ICottageRepository _repository;
        private readonly IMapper _mapper;

        public GetCottageByIdQueryHandler(ICottageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CottageDto> Handle(GetCottageByIdQuery request, CancellationToken cancellationToken)
        {
            var cottage = await _repository.GetById(request.Id);

            if (cottage == null)
                throw new NotFoundException("Cottage not found");

            return _mapper.Map<CottageDto>(cottage);
        }
    }
}