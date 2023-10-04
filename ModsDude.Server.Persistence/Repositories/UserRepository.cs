using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;


    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public async Task<bool> CheckUsernameTakenAsync(Username username)
    {
        return await _dbContext.Users.AnyAsync(user => user.Username == username);
    }
}
