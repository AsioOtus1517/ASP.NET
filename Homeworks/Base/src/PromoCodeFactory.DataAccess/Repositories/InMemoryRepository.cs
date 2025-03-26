﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateAsync(T entity) 
        {
            var newData = Data.ToList();

            newData.Add(entity);

            Data = newData;

            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(Guid id, T entity)
        {
            var newData = Data.ToList();
            var index = newData.FindIndex(x => x.Id == id);
            
            newData[index] = entity;

            Data = newData;

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guid id) 
        {
            Data = Data.Where(x => x.Id != id);

            return Task.CompletedTask;
        }
    }
}