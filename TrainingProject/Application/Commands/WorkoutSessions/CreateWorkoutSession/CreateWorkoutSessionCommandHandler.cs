using Application.DTOs;
using Application.Interfaces.RepositoryInterface;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.WorkoutSessions.CreateWorkoutSession
{
    public class CreateWorkoutSessionCommandHandler : IRequestHandler<CreateWorkoutSessionCommand, OperationResult<WorkoutSession>>
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<WorkoutSession> _sessions;
        private readonly ILogger<CreateWorkoutSessionCommandHandler> _logger;

        public CreateWorkoutSessionCommandHandler(
            IRepository<User> users,
            IRepository<WorkoutSession> sessions,
            ILogger<CreateWorkoutSessionCommandHandler> logger)
        {
            _users = users;
            _sessions = sessions;
            _logger = logger;
        }

        public async Task<OperationResult<WorkoutSession>> Handle(
            CreateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                return OperationResult<WorkoutSession>.Failure("UserId is required.");

            if (request.Session is null)
                return OperationResult<WorkoutSession>.Failure("Session payload is required.");

            if (request.Session.Exercises is null || !request.Session.Exercises.Any())
                return OperationResult<WorkoutSession>.Failure("At least one exercise is required.");

            var userResult = await _users.GetByIdAsync(request.UserId, cancellationToken);
            if (!userResult.IsSuccess || userResult.Data is null)
            {
                _logger.LogWarning("User not found. UserId={UserId}", request.UserId);
                return OperationResult<WorkoutSession>.Failure("User not found.");
            }

            var user = userResult.Data;

            var dto = request.Session;
            var session = new WorkoutSession(dto.Date, user, dto.Notes ?? string.Empty);

            foreach (var exercise in dto.Exercises)
            {
                switch (exercise.Type)
                {
                    case ExerciseType.Strength:
                        if (exercise.Sets is null || exercise.Reps is null || exercise.Weight is null)
                            return OperationResult<WorkoutSession>.Failure("Strength exercise requires Sets, Reps and Weight.");

                        session.Exercises.Add(new StrengthExercise(
                            exercise.Name,
                            exercise.Sets.Value,
                            exercise.Reps.Value,
                            exercise.Weight.Value));
                        break;

                    case ExerciseType.Cardio:
                        if (exercise.DistanceKm is null || exercise.Duration is null)
                            return OperationResult<WorkoutSession>.Failure("Cardio exercise requires DistanceKm and Duration.");

                        session.Exercises.Add(new CardioExercise(
                            exercise.Name,
                            exercise.DistanceKm.Value,
                            exercise.Duration.Value));
                        break;

                    default:
                        return OperationResult<WorkoutSession>.Failure("Unknown exercise type.");
                }
            }

            try
            {
                var addResult = await _sessions.AddAsync(session, cancellationToken);
                if (!addResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to add WorkoutSession. Error={Error}", addResult.ErrorMessage);
                    return OperationResult<WorkoutSession>.Failure(addResult.ErrorMessage ?? "Failed to add session.");
                }

                _logger.LogInformation("WorkoutSession {SessionId} added for User {UserId}", session.Id, user.Id);
                return OperationResult<WorkoutSession>.Success(session, "Workout session added.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding WorkoutSession for User {UserId}", user.Id);
                return OperationResult<WorkoutSession>.Failure("Unexpected error while saving session.");
            }
        }
    }
}
