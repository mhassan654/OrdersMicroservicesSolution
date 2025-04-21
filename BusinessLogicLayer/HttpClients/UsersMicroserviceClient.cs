using System.Net;
using System.Net.Http.Json;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.HttpClients;

public class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;

    public UsersMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDto?> GetUserById(Guid id)
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
                throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
            }
        }

        UserDto? user = await response.Content.ReadFromJsonAsync<UserDto>();

        if (user ==null)
        {
            throw new ArgumentException("User could not be found");
        }

        return user;

    }
}