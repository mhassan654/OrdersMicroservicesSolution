using System.Net;
using System.Net.Http.Json;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace BusinessLogicLayer.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;

    public UsersMicroserviceClient(HttpClient httpClient,
        ILogger<UsersMicroserviceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserById(Guid id)
    {
        try
        {

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
            
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad Request",null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    // throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                    return new UserDto(
                        PersonName: "Temporarily Unavailable",
                        Email: "Temporarily Unavailable",
                        Gender: "Temporarily Unavailable",
                        UserId: Guid.Empty);
                }
            }

            UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();

            if (user ==null)
            {
                throw new ArgumentException("User could not be found");
            }

            return user;
        }
        catch (BrokenCircuitException e)
        {
            // Console.WriteLine(e);
            // throw;
            _logger.LogError(e, "Request failed because of circuit breaker " +
                                "is in open state. Returning dummy data.");
            return new UserDto(
                PersonName: "Temporarily Unavailable",
                Email: "Temporarily Unavailable",
                Gender: "Temporarily Unavailable",
                UserId: Guid.Empty); 
        }
       

    }
}