using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Commands.UpdateVigenciaPromocion
{
    public class UpdateVigenciaPromocionCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
    public class UpdateVigenciaPromocionCommandHandler : IRequestHandler<UpdateVigenciaPromocionCommand, Response<Guid>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        public UpdateVigenciaPromocionCommandHandler(IPromocionRepositoryAsync productRepository)
        {
            _promocionRepository = productRepository;
        }
        public async Task<Response<Guid>> Handle(UpdateVigenciaPromocionCommand command, CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.GetByIdAsync(command.Id);

            if (promocion == null)
            {
                throw new ApiException($"Promoción no encontrada.");
            }
            else
            {
                promocion.FechaInicio = command.FechaInicio;
                promocion.FechaFin = command.FechaFin;
                promocion.FechaModificacion = DateTime.UtcNow;
                await _promocionRepository.UpdateAsync(promocion);
                return new Response<Guid>(promocion.Id);
            }
        }
    }
}
