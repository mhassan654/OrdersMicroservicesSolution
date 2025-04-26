using System.Text;
using System.Text.Json;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.Fallback;

namespace BusinessLogicLayer.Policies;

public class ProductsMicroservicePolicies: IProductsMicroservicePolicies
{
    private readonly ILogger<IProductsMicroservicePolicies> _logger;

    public ProductsMicroservicePolicies(ILogger<IProductsMicroservicePolicies> logger)
    {
        _logger = logger;
    }
    
    public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
    {

        AsyncFallbackPolicy<HttpResponseMessage> policy =
            Policy.HandleResult<HttpResponseMessage>(r
            => !r.IsSuccessStatusCode).FallbackAsync(async(context) =>
            {
                _logger.LogInformation("Fallback riggered: The request failed, returning dummy data");
                ProductDto product = new ProductDto(
                    ProductID: Guid.Empty,
                    ProductName: "Temporarily Unavailable",
                    Category: "Temporarily Unavailable",
                    UnitPrice: 0,
                    QuantityInStock: 0);

              var response=  new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(product),
                    Encoding.UTF8, "application/json")
                };

                return response;

            });
            return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy()
    {
      AsyncPolicy<HttpResponseMessage> policy= Policy.BulkheadAsync<HttpResponseMessage>(
           maxParallelization: 2, // number of parallel requests
            maxQueuingActions: 40, // number of queued requests
            onBulkheadRejectedAsync: (context) =>
            {
                _logger.LogInformation("Bulkhead isolation triggered: The request was rejected due to too many concurrent requests");
                throw new BulkheadRejectedException(
                    "Bulkhead isolation triggered: The request was rejected due to too many concurrent requests");
            });

        return policy;
    }
}