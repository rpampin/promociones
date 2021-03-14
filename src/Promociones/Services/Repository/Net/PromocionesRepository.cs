using MongoDB.Driver;
using Promociones.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Promociones.Services.Repository.Net
{
    public class PromocionesRepository : IPromocionesRepository
    {
        private readonly IMongoCollection<Promocion> _promociones;

        public PromocionesRepository(IBasePromociones settings)
        {
            var client = new MongoClient(settings.StringConnexion);
            var database = client.GetDatabase(settings.NombreBase);
            _promociones = database.GetCollection<Promocion>(settings.NombreColeccion);
        }

        public List<Promocion> Get() =>
            _promociones.Find(promocion => true).ToList();

        public Promocion Get(Guid id) =>
            _promociones.Find<Promocion>(promocion => promocion.Id == id).FirstOrDefault();

        public IQueryable<Promocion> Query() =>
            _promociones.AsQueryable<Promocion>();

        public Promocion Create(Promocion promocion)
        {
            _promociones.InsertOne(promocion);
            return promocion;
        }

        public void Update(Guid id, Promocion promocionIn) =>
            _promociones.ReplaceOne(promocion => promocion.Id == id, promocionIn);

        public void Remove(Guid id) =>
            _promociones.DeleteOne(promocion => promocion.Id == id);
    }
}
