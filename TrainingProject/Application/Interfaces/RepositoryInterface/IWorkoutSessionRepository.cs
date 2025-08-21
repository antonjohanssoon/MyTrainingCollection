using Domain;

namespace Application.Interfaces.RepositoryInterface
{
    public interface IWorkoutSessionRepository : IRepository<WorkoutSession>
    {
        Task<OperationResult<WorkoutSession?>> GetWithExercisesAsync(Guid sessionId, CancellationToken cancellationToken = default);
    }
}
