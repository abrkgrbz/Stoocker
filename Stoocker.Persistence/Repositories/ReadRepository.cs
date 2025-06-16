using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Domain.Common;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories
{
    public class ReadRepository<T>:IReadRepository<T> where T:BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        private bool _asNoTracking = false;

        public ReadRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Query().ToListAsync(cancellationToken);
        }

     

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().FirstOrDefaultAsync(predicate, cancellationToken);
        }
         
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await Query().CountAsync(cancellationToken)
                : await Query().CountAsync(predicate, cancellationToken);
        }

        


        public IReadRepository<T> AsNoTracking()
        {
            _asNoTracking = true;
            return this;
        }

        public IReadRepository<T> AsTracking()
        {
            _asNoTracking = false;
            return this;
        }
    }
}
