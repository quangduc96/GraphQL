global using GraphQL.Models;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;
global using GraphQL.Data;
global using GraphQL.GraphQL.Platforms;
global using GraphQL.GraphQL.Commands;
using Microsoft.Data.SqlClient;
using GraphQL.GraphQL;
using GraphQL.Server.Ui.Voyager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlBuilder = new SqlConnectionStringBuilder();
sqlBuilder.ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection");
sqlBuilder.UserID = builder.Configuration["UserId"];
sqlBuilder.Password = builder.Configuration["Password"];
builder.Services.AddPooledDbContextFactory<AppDbContext>(opt => opt.UseSqlServer(sqlBuilder.ConnectionString));
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<PlatformType>()
    .AddType<CommandType>()
    .AddFiltering()
    .AddSorting()
    .AddInMemorySubscriptions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.UseGraphQLVoyager(new VoyagerOptions()
{
    GraphQLEndPoint = "/graphql"
});

app.Run();