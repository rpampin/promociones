using System;
using System.Collections.Generic;

namespace Promociones.ViewModel
{
    public class PromocionPostViewModel
    {
        public IEnumerable<string> MediosDePago { get; set; }
        public IEnumerable<string> Bancos { get; set; }
        public IEnumerable<string> CategoriasProductos { get; set; }
        public int? MaximaCantidadDeCuotas { get; set; }
        public decimal? ValorInteresCuotas { get; set; }
        public decimal? PorcentajeDeDescuento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
