﻿using DataService.Infrastructure.Entities;
using DataWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataService.Core.Contracts
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

        Task<TEntity> GetById(
            int id,
            string includeProperties = "");

        Task<Result> Insert(TEntity entity);

        Task<Result> Delete(int id);

        Task<Result> Delete(TEntity entityToDelete);

        Task<Result> Update(TEntity entityToUpdate);

        //DbSet<TEntity> DbSet();

    }
}
