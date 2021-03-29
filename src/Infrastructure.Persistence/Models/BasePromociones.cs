namespace Infrastructure.Persistence.Models
{
    public class BasePromociones : IBasePromociones
    {
        public string NombreColeccion { get; set; }
        public string StringConnexion { get; set; }
        public string NombreBase { get; set; }
    }

    public interface IBasePromociones
    {
        string NombreColeccion { get; set; }
        string StringConnexion { get; set; }
        string NombreBase { get; set; }
    }
}
