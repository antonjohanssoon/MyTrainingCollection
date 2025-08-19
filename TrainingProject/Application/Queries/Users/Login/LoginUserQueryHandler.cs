using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.Users.Login.Helpers;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Users.Login
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, OperationResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginUserQueryHandler> _logger;
        private readonly TokenHelper _tokenHelper;

        public async Task<OperationResult<string>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _userRepository.GetByUsernameAsync(request.LoginUser.Username);

                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    _logger.LogWarning("Invalid login attempt for username: {Username}", request.LoginUser.Username);
                    return OperationResult<string>.Failure("Invalid username or password");
                }

                var user = userResult.Data;

                if (!BCrypt.Net.BCrypt.Verify(request.LoginUser.PasswordHash, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user: {Username}", request.LoginUser.Username);
                    return OperationResult<string>.Failure("Invalid password.", "Authentication error.");
                }

                string token = _tokenHelper.GenerateJwtToken(user);

                _logger.LogInformation("User {Username} successfully logged in.", request.LoginUser.Username);

                return OperationResult<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to log in user: {Username}", request.LoginUser.Username);
                return OperationResult<string>.Failure($"An error occurred: {ex.Message}");
            }
        }

    }
}
