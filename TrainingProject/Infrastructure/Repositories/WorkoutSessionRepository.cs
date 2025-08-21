using Application.Interfaces.RepositoryInterface;
using Domain;
using Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly Database _db;
        public WorkoutSessionRepository(Database db) => _db = db;

        public async Task<OperationResult<WorkoutSession>> GetWithExercisesAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var session = await _db.WorkoutSessions
                    .Include(workoutSession => workoutSession.Exercises)
                    .FirstOrDefaultAsync(workoutSession => workoutSession.Id == sessionId, cancellationToken);
                return OperationResult<WorkoutSession?>.Success(session);
            }
            catch (Exception ex)
            {
                return OperationResult<WorkoutSession?>.Failure($"Failed to load session. {ex.Message}");
            }
        }

        // ------- Återanvänd generiska metoder via IRepository<User> -------

        public async Task<OperationResult<WorkoutSession?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var entity = await _db.WorkoutSessions.FindAsync(new object?[] { id }, ct);
                return OperationResult<WorkoutSession?>.Success(entity);
            }
            catch (Exception ex)
            {
                return OperationResult<WorkoutSession?>.Failure($"Failed to get User with id {id}. {ex.Message}");
            }
        }

        public async Task<OperationResult<IReadOnlyList<WorkoutSession>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var list = await _db.WorkoutSessions.AsNoTracking().ToListAsync(ct);
                return OperationResult<IReadOnlyList<WorkoutSession>>.Success(list);
            }
            catch (Exception ex)
            {
                return OperationResult<IReadOnlyList<WorkoutSession>>.Failure($"Failed to get all Users. {ex.Message}");
            }
        }

        public async Task<OperationResult<WorkoutSession>> AddAsync(WorkoutSession entity, CancellationToken ct = default)
        {
            try
            {
                await _db.WorkoutSessions.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);
                return OperationResult<WorkoutSession>.Success(entity, "Added successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<WorkoutSession>.Failure($"Failed to add User. {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(WorkoutSession entity, CancellationToken ct = default)
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

        public async Task<OperationResult<bool>> DeleteAsync(WorkoutSession entity, CancellationToken ct = default)
        {
            try
            {
                _db.WorkoutSessions.Remove(entity);
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
