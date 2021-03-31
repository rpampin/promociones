using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Commands.DeletePromocionById
{
    public class DeletePromocionByIdCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
    }
    public class DeleteProductByIdCommandHandler : IRequestHandler<DeletePromocionByIdCommand, Response<Guid>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        public DeleteProductByIdCommandHandler(IPromocionRepositoryAsync promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }
        public async Task<Response<Guid>> Handle(DeletePromocionByIdCommand command, CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.GetByIdAsync(command.Id);
            if (promocion == null) throw new ApiException($"Promoción no encontrada.");
            promocion.Activo = false;
            await _promocionRepository.UpdateAsync(promocion);
            return new Response<Guid>(promocion.Id);
        }
    }
}
