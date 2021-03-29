using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Queries.GetPromocionesVigentesPorFecha
{
    public class GetPromocionesVigentesPorFechaQuery : IRequest<Response<IEnumerable<Promocion>>>
    {
        public DateTime Date { get; set; }
    }
    public class GetPromocionesVigentesPorFechaQueryHandler : IRequestHandler<GetPromocionesVigentesPorFechaQuery, Response<IEnumerable<Promocion>>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;

        public GetPromocionesVigentesPorFechaQueryHandler(IPromocionRepositoryAsync promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Response<IEnumerable<Promocion>>> Handle(GetPromocionesVigentesPorFechaQuery request, CancellationToken cancellationToken)
        {
            var promociones = await _promocionRepository.GetPromocionesVigentesPorFechaAsync(request.Date);
            return new Response<IEnumerable<Promocion>>(promociones);
        }
    }
}
