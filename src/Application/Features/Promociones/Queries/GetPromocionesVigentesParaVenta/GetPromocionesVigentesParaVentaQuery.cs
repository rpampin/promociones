using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta
{
    public class GetPromocionesVigentesParaVentaQuery : IRequest<Response<IEnumerable<PromocionesVigentesParaVentaModel>>>
    {
        public string MedioDePago { get; set; }
        public string Banco { get; set; }
        public IEnumerable<string> CategoriaProducto { get; set; }
    }
    public class GetPromocionesVigentesParaVentaQueryHandler : IRequestHandler<GetPromocionesVigentesParaVentaQuery, Response<IEnumerable<PromocionesVigentesParaVentaModel>>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        private readonly IMapper _mapper;

        public GetPromocionesVigentesParaVentaQueryHandler(IPromocionRepositoryAsync promocionRepository, IMapper mapper)
        {
            _promocionRepository = promocionRepository;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<PromocionesVigentesParaVentaModel>>> Handle(GetPromocionesVigentesParaVentaQuery request, CancellationToken cancellationToken)
        {
            var promociones = await _promocionRepository.GetPromocionesVigentesParaVentaAsync(request.MedioDePago, request.Banco, request.CategoriaProducto);
            var promocionesViewModel = _mapper.Map<IList<PromocionesVigentesParaVentaModel>>(promociones.ToList());
            return new Response<IEnumerable<PromocionesVigentesParaVentaModel>>(promocionesViewModel);
        }
    }
}
