using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace POC.Data.Repositories;

/// <summary>
/// Base repository implementing common database operations with proper DbContext management
/// </summary>
/// <typeparam name="TEntity">The entity type this repository handles</typeparam>
public abstract class BaseRepository<TEntity> : IDisposable where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    protected BaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected virtual IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>().AsNoTracking();
    }

    protected async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    protected async Task ExecuteInTransactionAsync(Func<Task> operation)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await GetQueryable().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetQueryable().Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        await ExecuteInTransactionAsync(async () =>
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await SaveChangesAsync();
        });
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));
        
        await ExecuteInTransactionAsync(async () =>
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            await SaveChangesAsync();
        });
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        await ExecuteInTransactionAsync(async () =>
        {
            _context.Set<TEntity>().Update(entity);
            await SaveChangesAsync();
        });
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        await ExecuteInTransactionAsync(async () =>
        {
            _context.Set<TEntity>().Remove(entity);
            await SaveChangesAsync();
        });
    }

    protected virtual async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Log the exception and rethrow
            throw new Exception("A concurrency error occurred while saving changes", ex);
        }
        catch (DbUpdateException ex)
        {
            // Log the exception and rethrow
            throw new Exception("An error occurred while saving changes to the database", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Context is scoped and will be disposed by the DI container
            // No need to dispose it here as it would cause issues with the scoped lifetime
        }
    }
}