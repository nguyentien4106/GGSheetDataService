using CleanArchitecture.Core.Entities;
using DataWorkerService.Models;
using System.Linq.Expressions;

namespace CleanAchitecture.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

        Task<TEntity> GetById(
            int id, 
            string includeProperties = "");

        Task<Result> Insert(TEntity entity);

        Task<Result> Delete(object id);

        Task<Result> Delete(TEntity entityToDelete);

        Task<Result> Update(TEntity entityToUpdate);
        
    }
}
