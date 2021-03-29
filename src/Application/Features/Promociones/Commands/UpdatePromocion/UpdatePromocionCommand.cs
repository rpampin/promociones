using Application.Exceptions;
using Application.Features.Commands;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Commands.UpdatePromocion
{
    public class UpdatePromocionCommand : IRequest<Response<Guid>>
    {
        public class UpdatePromocionCommandHandler : IRequestHandler<PromocionCommandModel, Response<Guid>>
        {
            private readonly IPromocionRepositoryAsync _promocionRepository;
            private readonly IMapper _mapper;
            public UpdatePromocionCommandHandler(IPromocionRepositoryAsync productRepository, IMapper mapper)
            {
                _promocionRepository = productRepository;
                _mapper = mapper;
            }
            public async Task<Response<Guid>> Handle(PromocionCommandModel command, CancellationToken cancellationToken)
            {
                var promocionExists = await _promocionRepository.ExistsAsync(command.Id);

                if (!promocionExists)
                {
                    throw new ApiException($"Promoción no encontrada.");
                }
                else
                {
                    var promocion = _mapper.Map<Promocion>(command);
                    await _promocionRepository.UpdateAsync(promocion);
                    return new Response<Guid>(promocion.Id);
                }
            }
        }
    }
}
