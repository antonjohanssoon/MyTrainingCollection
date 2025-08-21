using Domain;
using MediatR;

namespace Application.Commands.WorkoutSessions.DeleteExerciseInWorkoutSession
{
    public class DeleteExerciseInWorkoutSessionCommand : IRequest<OperationResult<bool>>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public Guid ExerciseId { get; set; }

        public DeleteExerciseInWorkoutSessionCommand(Guid userId, Guid sessionId, Guid exerciseId)
        {
            UserId = userId;
            SessionId = sessionId;
            ExerciseId = exerciseId;
        }
    }
}
