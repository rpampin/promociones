using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PromocionRepositoryAsync : GenericRepositoryAsync<Promocion>, IPromocionRepositoryAsync
    {
        public PromocionRepositoryAsync(IBasePromociones settings) : base(settings) { }

        FilterDefinition<Promocion> FilterActivePromociones(DateTime? date = null)
        {
            if (!date.HasValue)
                date = DateTime.UtcNow;
            
            var filterBuilder = Builders<Promocion>.Filter;

            var filterActive = filterBuilder.Eq(p => p.Activo, true);
            var filterDateFrom = filterBuilder.And(filterBuilder.Not(filterBuilder.Eq(p => p.FechaInicio, null)), filterBuilder.Lt(p => p.FechaInicio, date));
            var filterDateTo = filterBuilder.Or(filterBuilder.Eq(p => p.FechaFin, null), filterBuilder.Gt(p => p.FechaFin, date));

            return filterActive & filterDateFrom & filterDateTo;
        }

        public async Task<IReadOnlyList<Promocion>> GetPromocionesVigentesAsync()
        {
            return await _dbContext.Find(FilterActivePromociones()).ToListAsync();
        }

        public async Task<IReadOnlyList<Promocion>> GetPromocionesVigentesPorFechaAsync(DateTime date)
        {
            return await _dbContext.Find(FilterActivePromociones(date)).ToListAsync();
        }

        public async Task<IReadOnlyList<Promocion>> GetPromocionesVigentesParaVentaAsync(string MedioDePago, string Banco, IEnumerable<string> CategoriaProducto)
        {
            var filters = FilterActivePromociones();

            var filterBuilder = Builders<Promocion>.Filter;
            if (!string.IsNullOrEmpty(MedioDePago))
                filters &= filterBuilder.ElemMatch(p => p.MediosDePago, MedioDePago);
            if (!string.IsNullOrEmpty(MedioDePago))
                filters &= filterBuilder.ElemMatch(p => p.Bancos, Banco);
            if (CategoriaProducto != null && CategoriaProducto.Count() > 0)
                filters &= filterBuilder.AnyIn(p => p.CategoriasProductos, CategoriaProducto);

            return await _dbContext.Find(filters).ToListAsync();
        }

        public async Task<Promocion> GetPromocionSolapadaAsync(Promocion promocion)
        {
            var filterBuilder = Builders<Promocion>.Filter;
            var filterId = filterBuilder.Not(filterBuilder.Eq(p => p.Id, promocion.Id));
            var filterSolapada = filterBuilder.And(
                    filterBuilder.Or(
                        filterBuilder.Eq(p => p.FechaFin, null),
                        filterBuilder.Gt(p => p.FechaFin, promocion.FechaInicio ?? DateTime.MinValue)
                    ),
                    filterBuilder.Lt(p => p.FechaInicio, promocion.FechaFin ?? DateTime.MaxValue),
                    filterBuilder.Or(
                        filterBuilder.AnyIn(p => p.Bancos, promocion.Bancos),
                        filterBuilder.AnyIn(p => p.MediosDePago, promocion.MediosDePago),
                        filterBuilder.AnyIn(p => p.CategoriasProductos, promocion.CategoriasProductos)
                    )
                );

            return await _dbContext.Find(filterId & filterSolapada).FirstOrDefaultAsync();
        }
    }
}
