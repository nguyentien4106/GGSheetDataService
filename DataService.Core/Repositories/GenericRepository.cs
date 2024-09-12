using DataService.Core.Contracts;
using DataService.Infrastructure.Data;
using DataService.Infrastructure.Entities;
using DataWorkerService.Models;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataService.Core.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected DbSet<TEntity> dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual async Task<TEntity> GetByID(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<Result> Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(500, ex.Message);
            }
        }

        public virtual async Task<Result> Delete(int id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);

            return await Delete(entityToDelete);
        }

        public virtual async Task<Result> Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
            {
                return Result.Fail(404, "Object not found");
            }

            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public virtual async Task<Result> Update(TEntity entityToUpdate)
        {
            try
            {
                dbSet.Attach(entityToUpdate);
                _context.Entry(entityToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(500, ex.Message);
            }
        }

        public virtual async Task<TEntity> GetById(int id, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;


            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(item => item.Id == id);
        }
    }
}
