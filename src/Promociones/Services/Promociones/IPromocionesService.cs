using Promociones.Model;
using Promociones.ViewModel;
using System;
using System.Collections.Generic;

namespace Promociones.Services.Promociones
{
    public interface IPromocionesService
    {
        IList<PromocionViewModel> ObtenerPromociones();
        PromocionViewModel ObtenerPromocion(Guid id);
        List<PromocionViewModel> ObtenerPromocionesVigentes(DateTime? fecha = null);
        List<PromocionVentaViewModel> ObtenerPromocionesVigentesVenta(FiltroVentaViewModel filtro);
        PromocionViewModel CrearPromocion(PromocionPostViewModel promocionPostViewModel);
        PromocionViewModel ActualizarPromocion(Guid id, PromocionPostViewModel promocionPostViewModel);
        PromocionViewModel ActualizarVigenciaPromocion(Guid id, VigenciaViewModel vigenciaViewModel);
        PromocionViewModel EliminarPromocion(Guid id);
    }
}
