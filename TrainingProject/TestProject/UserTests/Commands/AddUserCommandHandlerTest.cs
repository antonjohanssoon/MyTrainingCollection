using Application.Commands.Users.CreateUser;
using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace TestProject.UsersTests.UsersCommandTests
{
    [TestFixture]
    public class AddNewUserCommandHandlerTests
    {
        private IUserRepository _userRepository;
        private ILogger<CreateUserCommandHandler> _logger;
        private CreateUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepository = A.Fake<IUserRepository>();
            _logger = A.Fake<ILogger<CreateUserCommandHandler>>();

            _handler = new CreateUserCommandHandler(_userRepository, _logger);

            // Default: användarnamn är INTE taget
            A.CallTo(() => _userRepository.UsernameExistsAsync(
                    A<string>._,
                    A<CancellationToken>._))
             .Returns(Task.FromResult(false));
        }

        [Test]
        public async Task Handle_ShouldAddUserSuccessfully_WhenRepositoryReturnsSuccess()
        {
            var newUserDto = new UserDTO { Username = "AntonJohansson", PasswordHash = "TestPassword123" };
            var command = new CreateUserCommand(newUserDto);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = newUserDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUserDto.PasswordHash)
            };

            A.CallTo(() => _userRepository.AddAsync(
                    A<User>.That.Matches(u => u.Username == newUser.Username),
                    A<CancellationToken>._))
             .Returns(Task.FromResult(OperationResult<User>.Success(newUser, "User added successfully.")));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("AntonJohansson", result.Data.Username);

            A.CallTo(() => _userRepository.UsernameExistsAsync("AntonJohansson", A<CancellationToken>._))
             .MustHaveHappenedOnceExactly();

            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExists()
        {
            var newUserDto = new UserDTO { Username = "duplicateexample", PasswordHash = "TestPassword123" };
            var command = new CreateUserCommand(newUserDto);

            A.CallTo(() => _userRepository.UsernameExistsAsync("duplicate", A<CancellationToken>._))
             .Returns(Task.FromResult(true));

            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .Returns(Task.FromResult(OperationResult<User>.Failure("A user with this Username already exists.", "Duplicate error.")));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("A user with this Username already exists.", result.ErrorMessage);
            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            var newUserDto = new UserDTO { Username = "error", PasswordHash = "TestPassword123" };
            var command = new CreateUserCommand(newUserDto);

            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .Throws(new Exception("Database connection failed."));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.ErrorMessage, Does.Contain("Database connection failed"));
            A.CallTo(() => _userRepository.UsernameExistsAsync("error", A<CancellationToken>._));
        }

        [Test]
        public async Task Handle_ShouldHashPasswordCorrectly_WhenAddingUser()
        {
            var newUserDto = new UserDTO { Username = "hashcheck", PasswordHash = "PlainTextPassword" };
            var command = new CreateUserCommand(newUserDto);

            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .ReturnsLazily(call =>
             {
                 var user = call.GetArgument<User>(0);
                 Assert.IsTrue(BCrypt.Net.BCrypt.Verify("PlainTextPassword", user.PasswordHash));
                 return Task.FromResult(OperationResult<User>.Success(user, "User added successfully."));
             });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("hashcheck", result.Data.Username);
            A.CallTo(() => _userRepository.AddAsync(A<User>.Ignored, A<CancellationToken>._))
             .MustHaveHappenedOnceExactly();
        }

    }
}

