using ModsDude.Server.Api.Schema.Roots;

var builder = WebApplication.CreateBuilder(args);


// TODO configure services

builder.Services
    .AddGraphQLServer()
    .AddQueryType<RootQuery>()
    .AddMutationType<RootMutation>()
    .AddMutationConventions();


var app = builder.Build();


app.MapGraphQL();


app.Run();
