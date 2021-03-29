using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPromocionRepositoryAsync : IGenericRepositoryAsync<Promocion>
    {
        Task<Promocion> GetPromocionSolapadaAsync(Promocion promocion);
        Task<IReadOnlyList<Promocion>> GetPromocionesVigentesAsync();
        Task<IReadOnlyList<Promocion>> GetPromocionesVigentesPorFechaAsync(DateTime date);
        Task<IReadOnlyList<Promocion>> GetPromocionesVigentesParaVentaAsync(string MedioDePago, string Banco, IEnumerable<string> CategoriaProducto);
    }
}
