using Application.Features.Promociones.Commands;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Promocion, PromocionCommandViewModel>();
            // CreateMap<Promocion, PromocionVentaViewModel>();
            // CreateMap<PromocionPostViewModel, Promocion>();
        }
    }
}
