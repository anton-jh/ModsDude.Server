using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Auth0.AuthenticationApi;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Middleware.UserLoading;
using ModsDude.Server.Application;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Application.Services;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Persistence.DbContexts;
using ModsDude.Server.Persistence.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    //options.ModelBinderProviders.Insert(0, new StronglyTypedIdModelBinderProvider());
    //options.Conventions.Add(new StronglyTypedIdConvention());
}).AddJsonOptions(options =>
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
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

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
    .AddScoped<IRepoService, RepoService>()
    .AddScoped<IProfileService, ProfileService>();

builder.Services
    .AddScoped<IRepoMembershipRepository, RepoMembershipRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IRepoRepository, RepoRepository>()
    .AddScoped<IProfileRepository, ProfileRepository>()
    .AddScoped<IModRepository, ModRepository>();

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
