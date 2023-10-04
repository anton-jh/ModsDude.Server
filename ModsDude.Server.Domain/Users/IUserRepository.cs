namespace ModsDude.Server.Domain.Users;
public interface IUserRepository
{
    public Task<bool> CheckUsernameTakenAsync(Username username);
    public void Add(User user);
}
