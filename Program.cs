using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAdvert.Api.HealthChecks;
using WebAdvert.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(new AWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();

// Add services to the container.


builder.Services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();

//inject automapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHealthChecks().AddCheck<StorageHealthChecks>("Storage");

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapHealthChecks("/healthz");

app.UseAuthorization();

app.MapControllers();




app.Run();
