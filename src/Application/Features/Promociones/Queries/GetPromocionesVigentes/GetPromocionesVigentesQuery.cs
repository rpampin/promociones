using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Queries.GetAllPromocionesVigentes
{
    public class GetPromocionesVigentesQuery : IRequest<Response<IEnumerable<Promocion>>>
    {
    }
    public class GetPromocionesVigentesHandler : IRequestHandler<GetPromocionesVigentesQuery, Response<IEnumerable<Promocion>>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        public GetPromocionesVigentesHandler(IPromocionRepositoryAsync promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Response<IEnumerable<Promocion>>> Handle(GetPromocionesVigentesQuery request, CancellationToken cancellationToken)
        {
            var promociones = await _promocionRepository.GetPromocionesVigentesAsync();
            return new Response<IEnumerable<Promocion>>(promociones.ToList());
        }
    }
}
