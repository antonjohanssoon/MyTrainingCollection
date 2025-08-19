using Application.Interfaces.RepositoryInterfaces;
using Domain;
using Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Database _db;

        public UserRepository(Database db) => _db = db;

        // ------- IUserRepository specifikt -------

        public async Task<OperationResult<User?>> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return OperationResult<User?>.Failure("Username is required.");

                var normalized = username.Trim().ToLower();
                var user = await _db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == normalized, ct);

                // Success även om null (ingen träff) – upp till anroparen att tolka
                return OperationResult<User?>.Success(user);
            }
            catch (Exception ex)
            {
                return OperationResult<User?>.Failure($"Failed to get user by username. {ex.Message}");
            }
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
        {
            var normalized = (username ?? string.Empty).Trim().ToLower();
            return await _db.Users.AnyAsync(u => u.Username.ToLower() == normalized, ct);
        }

        // ------- Återanvänd generiska metoder via IRepository<User> -------

        public async Task<OperationResult<User?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var entity = await _db.Users.FindAsync(new object?[] { id }, ct);
                return OperationResult<User?>.Success(entity);
            }
            catch (Exception ex)
            {
                return OperationResult<User?>.Failure($"Failed to get User with id {id}. {ex.Message}");
            }
        }

        public async Task<OperationResult<IReadOnlyList<User>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var list = await _db.Users.AsNoTracking().ToListAsync(ct);
                return OperationResult<IReadOnlyList<User>>.Success(list);
            }
            catch (Exception ex)
            {
                return OperationResult<IReadOnlyList<User>>.Failure($"Failed to get all Users. {ex.Message}");
            }
        }

        public async Task<OperationResult<User>> AddAsync(User entity, CancellationToken ct = default)
        {
            try
            {
                await _db.Users.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);
                return OperationResult<User>.Success(entity, "Added successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<User>.Failure($"Failed to add User. {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(User entity, CancellationToken ct = default)
        {
            try
            {
                _db.Entry(entity).State = EntityState.Modified;
                await _db.SaveChangesAsync(ct);
                return OperationResult<bool>.Success(true, "Updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return OperationResult<bool>.Failure($"Concurrency error updating User. {ex.Message}");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure($"Failed to update User. {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(User entity, CancellationToken ct = default)
        {
            try
            {
                _db.Users.Remove(entity);
                await _db.SaveChangesAsync(ct);
                return OperationResult<bool>.Success(true, "Deleted successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure($"Failed to delete User. {ex.Message}");
            }
        }
    }
}
