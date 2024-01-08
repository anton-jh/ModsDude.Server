using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Extensions;
using ModsDude.Server.Api.Schema.Roots;
using ModsDude.Server.Application;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Services;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;
using ModsDude.Server.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddMutationType<RootMutation>()
    .AddTypeExtensionsFromAssemblyContaining<Program>()
    .AddMutationConventions();

builder.Services
    .AddMediatR(config =>
        config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());

builder.Services
    .AddSingleton<ITimeService, TimeService>();

builder.Services
    .AddScoped<ISystemInviteRepository, SystemInviteRepository>()
    .AddScoped<IRepoMembershipRepository, RepoMembershipRepository>()
    .AddScoped<IUserRepository, UserRepository>();

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());


var app = builder.Build();


app.MapGraphQL();


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
        .Database.Migrate();
}


app.Run();
