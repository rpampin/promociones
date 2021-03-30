using Application.Features.Promociones.Commands.CreatePromocion;
using Application.Features.Promociones.Commands.UpdatePromocion;
using Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Promocion, CreatePromocionCommand>().ReverseMap();
            CreateMap<Promocion, UpdatePromocionCommand>().ReverseMap();
            CreateMap<Promocion, PromocionesVigentesParaVentaModel>();
        }
    }
}
