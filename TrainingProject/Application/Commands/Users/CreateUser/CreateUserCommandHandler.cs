using Application.Interfaces.RepositoryInterfaces;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Handling CreateUserCommand for user: {Name}", request.NewUser.Name);

            var taken = await _userRepository.UsernameExistsAsync(request.NewUser.Username, cancellationToken);
            if (taken)
            {
                return OperationResult<User>.Failure("Username already taken", "Validation error.");
            }

            var userToCreate = new User
            {
                Id = Guid.NewGuid(),
                Name = request.NewUser.Name,
                Username = request.NewUser.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewUser.PasswordHash)
            };


            try
            {
                var result = await _userRepository.AddAsync(userToCreate);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Successfully added a new user with username: {Username}", userToCreate.Username);
                    return OperationResult<User>.Success(userToCreate, "User added successfully!");
                }

                _logger.LogWarning("Failed to add user with username: {Username}. Error: {ErrorMessage}",
                       userToCreate.Username, result.ErrorMessage);

                return OperationResult<User>.Failure(result.ErrorMessage, "Failed to add user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user with username: {Username}", userToCreate.Username);
                return OperationResult<User>.Failure($"An error occurred: {ex.Message}", "Database error.");
            }
        }


    }
}
