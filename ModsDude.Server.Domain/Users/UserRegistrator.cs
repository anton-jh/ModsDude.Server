using ModsDude.Server.Domain.Exceptions;

namespace ModsDude.Server.Domain.Users;
public class UserRegistrator
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;


    public UserRegistrator(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }


    public async Task RegisterUser(Username username, Password password)
    {
        if (await _userRepository.CheckUsernameTakenAsync(username))
        {
            throw new UsernameTakenException();
        }

        PasswordHash hash = _passwordHasher.Hash(password);

        User user = new(username, hash);

        _userRepository.Add(user);
    }
}
