using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Auth0.AuthenticationApi;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Middleware.UserLoading;
using ModsDude.Server.Application;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Application.Services;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;
using ModsDude.Server.Persistence.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>();
    });

builder.Services
    .AddOpenApiDocument();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Auth:Authority");
        options.Audience = builder.Configuration.GetValue<string>("Auth:Audience");
        options.MapInboundClaims = false;
    });
builder.Services.AddAuthorization(options
    => options.AddApplicationPolicies());

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UserLoadingMiddleware>();

builder.Services.AddHttpClient<Auth0AuthenticationApiClient>();

builder.Services
    .AddSingleton<ITimeService, TimeService>()
    .AddTransient<IRepoAuthorizationService, RepoAuthorizationService>();

builder.Services
    .AddScoped<IRepoService, RepoService>();

builder.Services
    .AddScoped<IRepoMembershipRepository, RepoMembershipRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IRepoRepository, RepoRepository>();

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserLoadingMiddleware>();

app.MapControllers()
    .RequireAuthorization();


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
        .Database.Migrate();
}


app.Run();
