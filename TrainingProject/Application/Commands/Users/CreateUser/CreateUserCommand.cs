using Application.DTOs;
using Domain;
using MediatR;

namespace Application.Commands.Users.CreateUser
{
    public class CreateUserCommand : IRequest<OperationResult<User>>
    {
        public CreateUserCommand(UserDTO newUser)
        {
            NewUser = newUser;
        }

        public UserDTO NewUser { get; set; }
    }
}
