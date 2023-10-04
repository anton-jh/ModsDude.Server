using ModsDude.Server.Api.Schema;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddMutationType<RootMutation>()
    .AddMutationConventions();


var app = builder.Build();


app.MapGraphQL();


app.Run();
