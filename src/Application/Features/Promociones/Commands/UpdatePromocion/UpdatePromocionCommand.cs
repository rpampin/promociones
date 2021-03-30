using Application.Exceptions;
using Application.Features.Commands;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Commands.UpdatePromocion
{
    public class UpdatePromocionCommand : PromocionCommandModel
    {
        public Guid Id { get; set; }

    }
    public class UpdatePromocionCommandHandler : IRequestHandler<UpdatePromocionCommand, Response<Guid>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        private readonly IMapper _mapper;
        public UpdatePromocionCommandHandler(IPromocionRepositoryAsync productRepository, IMapper mapper)
        {
            _promocionRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<Response<Guid>> Handle(UpdatePromocionCommand command, CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.GetByIdAsync(command.Id);

            if (promocion == null)
            {
                throw new ApiException($"Promoción no encontrada.");
            }
            else
            {
                promocion = _mapper.Map(command, promocion);
                await _promocionRepository.UpdateAsync(promocion);
                return new Response<Guid>(promocion.Id);
            }
        }
    }
}
