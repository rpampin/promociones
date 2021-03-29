using System;
using System.Collections.Generic;

namespace Application.Features.Promociones.Queries.GetPromocionesVigentesParaVenta
{
    public class PromocionesVigentesParaVentaModel
    {
        public Guid Id { get; set; }
        public IEnumerable<string> MediosDePago { get; set; }
        public IEnumerable<string> Bancos { get; set; }
        public IEnumerable<string> CategoriasProductos { get; set; }
        public int? MaximaCantidadDeCuotas { get; set; }
        public decimal? ValorInteresCuotas { get; set; }
        public decimal? PorcentajeDeDescuento { get; set; }
    }
}
