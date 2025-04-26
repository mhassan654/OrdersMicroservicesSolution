

using BusinessLogicLayer;
using BusinessLogicLayer.HttpClients;
using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.Policies;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroservices.API.Middleware;
using Polly;

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
        builder.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddTransient<IUserMicroservicePolicies, UsersMicroservicePolicies>();
builder.Services.AddTransient<IProductsMicroservicePolicies, ProductsMicroservicePolicies>();
builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();

builder.Services.AddHttpClient<UsersMicroserviceClient>(
        client => client.BaseAddress = new Uri(
            $"http://{builder.Configuration["UsersMicroserviceName"]}:" +
            $"{builder.Configuration["UsersMicroservicePort"]}"))
       .AddPolicyHandler(
        builder.Services.BuildServiceProvider
                ().GetRequiredService<IUserMicroservicePolicies>
                ().GetCombinedPolicy())
    ;

//product base utl mapping
builder.Services.AddHttpClient<ProductsMicroserviceClient>(
    client => client.BaseAddress = new Uri(
        $"http://{builder.Configuration["ProductsMicroserviceName"]}:" +
        $"{builder.Configuration["ProductsMicroservicePort"]}"))

    .AddPolicyHandler(
        builder.Services.BuildServiceProvider
                ().GetRequiredService<IProductsMicroservicePolicies>
                ().GetFallbackPolicy())
    .AddPolicyHandler(
        builder.Services.BuildServiceProvider
                ().GetRequiredService<IProductsMicroservicePolicies>
                ().GetBulkheadIsolationPolicy())
    ;



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
