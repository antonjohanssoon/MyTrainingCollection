using Application.Interfaces.RepositoryInterface;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.WorkoutSessions.DeleteWorkoutSession
{
    public class DeleteWorkoutSessionCommandHandler : IRequestHandler<DeleteWorkoutSessionCommand, OperationResult<bool>>
    {
        private readonly IRepository<WorkoutSession> _workoutSessions;
        private readonly ILogger<DeleteWorkoutSessionCommandHandler> _logger;

        public DeleteWorkoutSessionCommandHandler(
            IRepository<WorkoutSession> workoutSessions,
            ILogger<DeleteWorkoutSessionCommandHandler> logger)
        {
            _workoutSessions = workoutSessions;
            _logger = logger;
        }

        public async Task<OperationResult<bool>> Handle(DeleteWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.SessionId == Guid.Empty)
                return OperationResult<bool>.Failure("SessionId is required.");

            var sessionRes = await _workoutSessions.GetByIdAsync(request.SessionId, cancellationToken);
            if (!sessionRes.IsSuccess || sessionRes.Data is null)
                return OperationResult<bool>.Failure("Workout session not found.");

            var session = sessionRes.Data;

            // Behörighetskontroll
            if (session.UserId != request.UserId)
                return OperationResult<bool>.Failure("You are not allowed to delete this session.");

            try
            {
                var deleteResult = await _workoutSessions.DeleteAsync(session, cancellationToken);
                if (!deleteResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete WorkoutSession {SessionId}. Error={Error}", session.Id, deleteResult.ErrorMessage);
                    return OperationResult<bool>.Failure(deleteResult.ErrorMessage ?? "Failed to delete session.");
                }

                _logger.LogInformation("Deleted WorkoutSession {SessionId} by User {UserId}", session.Id, request.UserId);
                return OperationResult<bool>.Success(true, "Workout session deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting WorkoutSession {SessionId}", session.Id);
                return OperationResult<bool>.Failure("Unexpected error while deleting session.");
            }
        }
    }
}
