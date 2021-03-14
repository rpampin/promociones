using AutoMapper;
using Promociones.Model;
using Promociones.ViewModel;

namespace Promociones.MappingConfigurations
{
    public class PromocionProfile : Profile
    {
        public PromocionProfile()
        {
            // Default mapping when property names are same
            CreateMap<Promocion, PromocionViewModel>();
            CreateMap<Promocion, PromocionVentaViewModel>();
            CreateMap<PromocionPostViewModel, Promocion>();
        }
    }
}
