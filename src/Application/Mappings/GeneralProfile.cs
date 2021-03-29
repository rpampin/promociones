using Application.Features.Commands;
using Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Promocion, PromocionCommandModel>();
            CreateMap<Promocion, PromocionesVigentesParaVentaModel>();
        }
    }
}
