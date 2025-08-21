using Application.Interfaces.RepositoryInterface;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.WorkoutSessions.DeleteExerciseInWorkoutSession
{
    public class DeleteExerciseInWorkoutSessionCommandHandler : IRequestHandler<DeleteExerciseInWorkoutSessionCommand, OperationResult<bool>>
    {
        private readonly IWorkoutSessionRepository _workoutSessions;
        private readonly ILogger<DeleteExerciseInWorkoutSessionCommandHandler> _logger;

        public DeleteExerciseInWorkoutSessionCommandHandler(IWorkoutSessionRepository workoutSessions, ILogger<DeleteExerciseInWorkoutSessionCommandHandler> logger)
        {
            _workoutSessions = workoutSessions;
            _logger = logger;
        }

        public async Task<OperationResult<bool>> Handle(DeleteExerciseInWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionRes = await _workoutSessions.GetWithExercisesAsync(request.SessionId, cancellationToken);
            if (!sessionRes.IsSuccess || sessionRes.Data is null)
                return OperationResult<bool>.Failure("Workout session not found.");

            var session = sessionRes.Data;

            if (session.UserId != request.UserId)
                return OperationResult<bool>.Failure("You are not allowed to modify this session.");

            var exercise = session.Exercises.FirstOrDefault(e => e.Id == request.ExerciseId);
            if (exercise is null)
                return OperationResult<bool>.Failure("Exercise not found.");

            session.Exercises.Remove(exercise);

            var saveResult = await _workoutSessions.UpdateAsync(session, cancellationToken);
            if (!saveResult.IsSuccess)
                return OperationResult<bool>.Failure(saveResult.ErrorMessage ?? "Failed to remove exercise.");

            _logger.LogInformation("Removed Exercise {ExerciseId} from Session {SessionId} by User {UserId}",
                request.ExerciseId, request.SessionId, request.UserId);

            return OperationResult<bool>.Success(true, "Exercise removed.");
        }
    }
}
