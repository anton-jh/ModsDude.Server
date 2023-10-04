using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Persistence.DbContexts;
public class ApplicationDbContext : DbContext
{
    public required DbSet<User> Users { get; init; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: Conversion for value objects
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
