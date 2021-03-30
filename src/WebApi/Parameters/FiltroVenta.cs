using System.Collections.Generic;

namespace WebApi.Parameters
{
    public class FiltroVenta
    {
        public string MedioDePago { get; set; }
        public string Banco { get; set; }
        public IEnumerable<string> CategoriaProducto { get; set; }
    }
}
