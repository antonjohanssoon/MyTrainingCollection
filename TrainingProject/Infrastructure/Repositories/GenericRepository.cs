using Application.Interfaces.RepositoryInterface;
using Domain;
using Infrastructure.Databases;

namespace Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;

    namespace Infrastructure.Repositories
    {
        public class GenericRepository<T> : IRepository<T> where T : class
        {
            private readonly Database _db;
            private readonly DbSet<T> _set;

            public GenericRepository(Database db)
            {
                _db = db;
                _set = _db.Set<T>();
            }

            public async Task<OperationResult<T?>> GetByIdAsync(Guid id, CancellationToken ct = default)
            {
                try
                {
                    var entity = await _set.FindAsync(new object?[] { id }, ct);
                    return OperationResult<T?>.Success(entity);
                }
                catch (Exception ex)
                {
                    return OperationResult<T?>.Failure($"Failed to get {typeof(T).Name} with id {id}. {ex.Message}");
                }
            }

            public async Task<OperationResult<IReadOnlyList<T>>> GetAllAsync(CancellationToken ct = default)
            {
                try
                {
                    var list = await _set.AsNoTracking().ToListAsync(ct);
                    return OperationResult<IReadOnlyList<T>>.Success(list);
                }
                catch (Exception ex)
                {
                    return OperationResult<IReadOnlyList<T>>.Failure($"Failed to get all {typeof(T).Name}. {ex.Message}");
                }
            }

            public async Task<OperationResult<T>> AddAsync(T entity, CancellationToken ct = default)
            {
                try
                {
                    await _set.AddAsync(entity, ct);
                    await _db.SaveChangesAsync(ct);
                    return OperationResult<T>.Success(entity, "Added successfully.");
                }
                catch (Exception ex)
                {
                    return OperationResult<T>.Failure($"Failed to add {typeof(T).Name}. {ex.Message}");
                }
            }

            public async Task<OperationResult<bool>> UpdateAsync(T entity, CancellationToken ct = default)
            {
                try
                {
                    _db.Entry(entity).State = EntityState.Modified;
                    await _db.SaveChangesAsync(ct);
                    return OperationResult<bool>.Success(true, "Updated successfully.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return OperationResult<bool>.Failure($"Concurrency error updating {typeof(T).Name}. {ex.Message}");
                }
                catch (Exception ex)
                {
                    return OperationResult<bool>.Failure($"Failed to update {typeof(T).Name}. {ex.Message}");
                }
            }

            public async Task<OperationResult<bool>> DeleteAsync(T entity, CancellationToken ct = default)
            {
                try
                {
                    _set.Remove(entity);
                    await _db.SaveChangesAsync(ct);
                    return OperationResult<bool>.Success(true, "Deleted successfully.");
                }
                catch (Exception ex)
                {
                    return OperationResult<bool>.Failure($"Failed to delete {typeof(T).Name}. {ex.Message}");
                }
            }
        }
    }

}
