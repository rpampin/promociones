using Application.Interfaces.Repositories;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Queries.GetAllPromociones
{
    public class GetAllPromocionesQuery : IRequest<Response<IEnumerable<Promocion>>>
    {
    }
    public class GetAllPromocionesQueryHandler : IRequestHandler<GetAllPromocionesQuery, Response<IEnumerable<Promocion>>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        public GetAllPromocionesQueryHandler(IPromocionRepositoryAsync promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Response<IEnumerable<Promocion>>> Handle(GetAllPromocionesQuery request, CancellationToken cancellationToken)
        {
            var promociones = await _promocionRepository.GetAllAsync();
            return new Response<IEnumerable<Promocion>>(promociones.ToList());
        }
    }
}
