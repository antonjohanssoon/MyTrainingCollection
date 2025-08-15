using Application.DTOs;
using Domain;
using MediatR;

namespace Application.Queries.Users.Login
{
    public class LoginUserQuery : IRequest<OperationResult<string>>
    {
        public LoginUserQuery(UserDTO loginUser)
        {
            LoginUser = loginUser;
        }

        public UserDTO LoginUser { get; set; }
    }


}
