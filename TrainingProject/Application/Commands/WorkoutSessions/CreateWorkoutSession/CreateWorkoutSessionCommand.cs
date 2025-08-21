using Application.DTOs;
using Domain;
using MediatR;

namespace Application.Commands.WorkoutSessions.CreateWorkoutSession
{
    public class CreateWorkoutSessionCommand : IRequest<OperationResult<WorkoutSession>>
    {
        public Guid UserId { get; }
        public WorkoutSessionDTO Session { get; }
    }
}
