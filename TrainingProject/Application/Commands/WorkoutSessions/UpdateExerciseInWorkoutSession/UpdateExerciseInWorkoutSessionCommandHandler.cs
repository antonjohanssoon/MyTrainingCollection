using Application.Commands.WorkoutSessions.UpdateExerciseInWorkoutSession;
using Application.DTOs;
using Application.Interfaces.RepositoryInterface;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdateExerciseInWorkoutSessionCommandHandler : IRequestHandler<UpdateExerciseInWorkoutSessionCommand, OperationResult<WorkoutSession>>
{
    private readonly IWorkoutSessionRepository _workoutSessions;
    private readonly ILogger<UpdateExerciseInWorkoutSessionCommandHandler> _logger;

    public UpdateExerciseInWorkoutSessionCommandHandler(IWorkoutSessionRepository sessions, ILogger<UpdateExerciseInWorkoutSessionCommandHandler> logger)
    {
        _workoutSessions = sessions;
        _logger = logger;
    }

    public async Task<OperationResult<WorkoutSession>> Handle(UpdateExerciseInWorkoutSessionCommand request, CancellationToken ct)
    {
        // Ladda session + exercises
        var sessionRes = await _workoutSessions.GetWithExercisesAsync(request.SessionId, ct);
        if (!sessionRes.IsSuccess || sessionRes.Data is null)
            return OperationResult<WorkoutSession>.Failure("Workout session not found.");

        var session = sessionRes.Data;

        // Behörighet
        if (session.UserId != request.UserId)
            return OperationResult<WorkoutSession>.Failure("You are not allowed to modify this session.");

        // Hitta exercise
        var exercise = session.Exercises.FirstOrDefault(e => e.Id == request.ExerciseId);
        if (exercise is null)
            return OperationResult<WorkoutSession>.Failure("Exercise not found in the session.");

        // Om typ byts: ta bort och skapa ny subklass
        if ((exercise is StrengthExercise && request.Update.Type == ExerciseType.Cardio) ||
            (exercise is CardioExercise && request.Update.Type == ExerciseType.Strength))
        {
            session.Exercises.Remove(exercise);

            if (request.Update.Type == ExerciseType.Strength)
            {
                if (request.Update.Sets is null || request.Update.Reps is null || request.Update.Weight is null)
                    return OperationResult<WorkoutSession>.Failure("Strength requires Sets, Reps, Weight.");
                session.Exercises.Add(new StrengthExercise(
                    request.Update.Name.Trim(), request.Update.Sets.Value, request.Update.Reps.Value, request.Update.Weight.Value));
            }
            else
            {
                if (request.Update.DistanceKm is null || request.Update.Duration is null)
                    return OperationResult<WorkoutSession>.Failure("Cardio requires DistanceKm and Duration.");
                session.Exercises.Add(new CardioExercise(
                    request.Update.Name.Trim(), request.Update.DistanceKm.Value, request.Update.Duration.Value));
            }
        }
        else
        {
            // Samma typ: uppdatera fält
            exercise.Name = request.Update.Name.Trim();

            if (exercise is StrengthExercise strengthExercise)
            {
                if (request.Update.Sets is null || request.Update.Reps is null || request.Update.Weight is null)
                    return OperationResult<WorkoutSession>.Failure("Strength requires Sets, Reps, Weight.");
                strengthExercise.Sets = request.Update.Sets.Value;
                strengthExercise.Reps = request.Update.Reps.Value;
                strengthExercise.Weight = request.Update.Weight.Value;
            }
            else if (exercise is CardioExercise cardioExercise)
            {
                if (request.Update.DistanceKm is null || request.Update.Duration is null)
                    return OperationResult<WorkoutSession>.Failure("Cardio requires DistanceKm and Duration.");
                cardioExercise.DistanceKm = request.Update.DistanceKm.Value;
                cardioExercise.Duration = request.Update.Duration.Value;
            }
        }

        // Spara
        var saveResult = await _workoutSessions.UpdateAsync(session, ct);
        if (!saveResult.IsSuccess)
            return OperationResult<WorkoutSession>.Failure(saveResult.ErrorMessage ?? "Failed to update exercise.");

        _logger.LogInformation("Updated Exercise {ExerciseId} in Session {SessionId} by User {UserId}",
            request.ExerciseId, request.SessionId, request.UserId);

        return OperationResult<WorkoutSession>.Success(session, "Exercise updated.");
    }
}

