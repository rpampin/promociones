using Application.Features.Commands;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Commands.CreatePromocion
{
    public class CreatePromocionCommand : PromocionCommandModel
    {
    }
    public class CreatePromocionCommandHandler : IRequestHandler<CreatePromocionCommand, Response<Guid>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        private readonly IMapper _mapper;
        public CreatePromocionCommandHandler(IPromocionRepositoryAsync promocionRepository, IMapper mapper)
        {
            _promocionRepository = promocionRepository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreatePromocionCommand request, CancellationToken cancellationToken)
        {
            var promocion = _mapper.Map<Promocion>(request);
            await _promocionRepository.AddAsync(promocion);
            return new Response<Guid>(promocion.Id);
        }
    }
}