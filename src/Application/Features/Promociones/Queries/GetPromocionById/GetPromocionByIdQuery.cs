using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Queries.GetPromocionById
{
    public class GetPromocionByIdQuery : IRequest<Response<Promocion>>
    {
        public Guid Id { get; set; }
        public class GetProductByIdQueryHandler : IRequestHandler<GetPromocionByIdQuery, Response<Promocion>>
        {
            private readonly IPromocionRepositoryAsync _promocionRepository;
            public GetProductByIdQueryHandler(IPromocionRepositoryAsync propromocionRepository)
            {
                _promocionRepository = propromocionRepository;
            }
            public async Task<Response<Promocion>> Handle(GetPromocionByIdQuery query, CancellationToken cancellationToken)
            {
                var promocion = await _promocionRepository.GetByIdAsync(query.Id);
                if (promocion == null) throw new ApiException($"Promoción no encontrada.");
                return new Response<Promocion>(promocion);
            }
        }
    }
}
