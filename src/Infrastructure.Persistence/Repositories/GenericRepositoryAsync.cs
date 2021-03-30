using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _dbContext;

        public GenericRepositoryAsync(IOptions<BasePromociones> basePromocionesOption)
        {
            var client = new MongoClient(basePromocionesOption.Value.StringConnexion);
            var database = client.GetDatabase(basePromocionesOption.Value.NombreBase);
            _dbContext = database.GetCollection<T>(basePromocionesOption.Value.NombreColeccion);
        }

        public async Task<bool> ExistsAsync(Guid Id)
        {
            return await _dbContext.CountDocumentsAsync(o => o.Id == Id) > 0;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.InsertOneAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            await _dbContext.DeleteOneAsync(o => o.Id == entity.Id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Find(o => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await _dbContext.ReplaceOneAsync(o => o.Id == entity.Id, entity);
        }
    }
}
