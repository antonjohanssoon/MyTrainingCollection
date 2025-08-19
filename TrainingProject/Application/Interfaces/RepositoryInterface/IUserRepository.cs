using Application.Interfaces.RepositoryInterface;
using Domain;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<OperationResult<User?>> GetByUsernameAsync(string username, CancellationToken ct = default);

        Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default);
    }
}

