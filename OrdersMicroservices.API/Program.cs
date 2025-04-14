

using BusinessLogicLayer;
using BusinessLogicLayer.HttpClients;
using BusinessLogicLayer.Mappers;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroservices.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

builder.Services.AddControllers();


//fluent validations
builder.Services.AddFluentValidationAutoValidation();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cors 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient<UsersMicroserviceClient>(
    client => client.BaseAddress = new Uri(
        $"http://{builder.Configuration["UsersMicroserviceName"]}:" +
        $"{builder.Configuration["UsersMicroservicePort"]}"));

// builder.Services.AddHttpClient<UsersMicroserviceClient>(
//     client => client.BaseAddress = new Uri("http://localhost:5209"));

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//cors
app.UseCors();

//Swagger
app.UseSwagger();
app.UseSwaggerUI(); 

//Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// controller routes
app.MapControllers();

app.Run();
