using System.Collections.Generic;

namespace Promociones.ViewModel
{
    public class FiltroVentaViewModel
    {
        public string MedioDePago { get; set; }
        public string Banco { get; set; }
        public IEnumerable<string> CategoriaProducto { get; set; }
    }
}
