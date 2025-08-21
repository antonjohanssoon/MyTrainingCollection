using Application.DTOs;
using Domain;
using MediatR;

namespace Application.Commands.WorkoutSessions.UpdateExerciseInWorkoutSession
{
    public class UpdateExerciseInWorkoutSessionCommand : IRequest<OperationResult<WorkoutSession>>
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }
        public Guid ExerciseId { get; }           // viktigt för att hitta exakt exercise
        public ExerciseDTO Update { get; }

        public UpdateExerciseInWorkoutSessionCommand(Guid userId, Guid sessionId, Guid exerciseId, ExerciseDTO update)
        {
            UserId = userId;
            SessionId = sessionId;
            ExerciseId = exerciseId;
            Update = update;
        }
    }
}
