using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Promociones.Model
{
    public class Promocion
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public IEnumerable<string> MediosDePago { get; set; }
        public IEnumerable<string> Bancos { get; set; }
        public IEnumerable<string> CategoriasProductos { get; set; }
        public int? MaximaCantidadDeCuotas { get; set; }
        public decimal? ValorInteresCuotas { get; set; }
        public decimal? PorcentajeDeDescuento { get; set; }
        [BsonDateTimeOptions]
        public DateTime? FechaInicio { get; set; }
        [BsonDateTimeOptions]
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        [BsonDateTimeOptions]
        public DateTime FechaCreacion { get; set; }
        [BsonDateTimeOptions]
        public DateTime? FechaModificacion { get; set; }
    }
}
