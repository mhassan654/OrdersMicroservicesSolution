

using BusinessLogicLayer;
using BusinessLogicLayer.Mappers;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroservices.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

builder.Services.AddControllers();

// builder.Services.AddAutoMapper(
//     typeof(OrderAddRequestToOrderMappingProfile).Assembly));

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


app.Run();
