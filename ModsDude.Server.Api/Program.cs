using ModsDude.Server.Api.Schema.Roots;
using ModsDude.Server.Application;
using ModsDude.Server.Application.Services;
using ModsDude.Server.Application.Users;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddMutationType<RootMutation>()
    .AddMutationConventions();

builder.Services
    .AddMediatR(config =>
        config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());

builder.Services
    .AddScoped<IJwtService, JwtService>()
    .AddSingleton<ITimeService, TimeService>();

builder.Services
    .AddScoped<LoginService>()
    .AddSingleton<IPasswordHasher, PasswordHasher>()
    .AddScoped<UserRegistrator>();

builder.Services
    .AddOptions<UsersOptions>()
    .BindConfiguration("Features:Users")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddScoped<ISystemInviteRepository, SystemInviteRepository>()
    .AddScoped<IRepoMembershipRepository, RepoMembmershipRepository>()
    .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
    .AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();


app.MapGraphQL();


app.Run();
