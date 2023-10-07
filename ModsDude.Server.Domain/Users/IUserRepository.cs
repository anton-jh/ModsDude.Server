namespace ModsDude.Server.Domain.Users;
public interface IUserRepository
{
    public Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default);
    public Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    public Task<bool> CheckUsernameTakenAsync(Username username, CancellationToken cancellationToken = default);
    public void Add(User user);
}
