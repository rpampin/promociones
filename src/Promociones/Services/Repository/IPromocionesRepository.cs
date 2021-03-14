using Promociones.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Promociones.Services.Repository
{
    public interface IPromocionesRepository
    {
        List<Promocion> Get();
        Promocion Get(Guid id);
        IQueryable<Promocion> Query();
        Promocion Create(Promocion promocion);
        void Update(Guid id, Promocion promocionIn);
        void Remove(Guid id);
    }
}
