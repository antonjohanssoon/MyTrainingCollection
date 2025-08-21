using Domain;
using MediatR;

namespace Application.Commands.WorkoutSessions.DeleteWorkoutSession
{
    public class DeleteWorkoutSessionCommand : IRequest<OperationResult<bool>>
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }

        public DeleteWorkoutSessionCommand(Guid userId, Guid sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
        }
    }
}
