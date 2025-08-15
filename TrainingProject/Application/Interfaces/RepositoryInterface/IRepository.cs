using Domain;

namespace Application.Interfaces.RepositoryInterface
{
    public interface IRepository<T> where T : class
    {
        Task<OperationResult<T?>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<OperationResult<T?>> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<OperationResult<IReadOnlyList<T>>> GetAllAsync(CancellationToken ct = default);
        Task<OperationResult<T>> AddAsync(T entity, CancellationToken ct = default);
        Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken ct = default);
        Task<OperationResult<T>> DeleteAsync(T entity, CancellationToken ct = default);

    }
}
