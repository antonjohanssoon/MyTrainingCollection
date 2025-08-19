using Domain;

namespace Application.Interfaces.RepositoryInterface
{
    public interface IRepository<T> where T : class
    {
        Task<OperationResult<T?>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<OperationResult<IReadOnlyList<T>>> GetAllAsync(CancellationToken ct = default);
        Task<OperationResult<T>> AddAsync(T entity, CancellationToken ct = default);
        Task<OperationResult<bool>> UpdateAsync(T entity, CancellationToken ct = default);
        Task<OperationResult<bool>> DeleteAsync(T entity, CancellationToken ct = default);

    }
}
