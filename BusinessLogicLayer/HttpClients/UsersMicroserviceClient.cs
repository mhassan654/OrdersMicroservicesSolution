using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace BusinessLogicLayer.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;
    private readonly IDistributedCache _distributedCache;

    public UsersMicroserviceClient(HttpClient httpClient,
        ILogger<UsersMicroserviceClient> logger,IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<UserDto?> GetUserById(Guid id)
    {
        try
        {
            string cachedKeyToRead = $"user:{id}";
            string cachedUser = await _distributedCache.GetStringAsync(cachedKeyToRead);

            if (cachedUser != null)
            {
                //deserialize the cached user
                UserDto? userFromCache =
                JsonSerializer.Deserialize<UserDto>(cachedUser);

                return userFromCache;
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    UserDto? fallbackUser = await response.Content.ReadFromJsonAsync<UserDto>();

                    if (fallbackUser ==null)
                    {
                        throw new NotImplementedException();
                    }

                    return fallbackUser;
                }

                else if (response.StatusCode == HttpStatusCode.NotFound)
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

            // store the user data (received from response) into cache
            string cacheKey = $"user:{id}";
            string userJson = JsonSerializer.Serialize(user);
            DistributedCacheEntryOptions options =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

            await _distributedCache.SetStringAsync(cacheKey, userJson, options);

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