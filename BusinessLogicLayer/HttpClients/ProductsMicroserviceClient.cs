using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BusinessLogicLayer.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public ProductsMicroserviceClient(
            HttpClient httpClient,
            ILogger<ProductsMicroserviceClient> logger,
            IDistributedCache distributedCache
            )
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<ProductDto?> GetProductById(Guid id)
        {
            try
            {
                //Key: product:123
                // Value: {"PRoductName:"...",..."}
                string cacheKey = $"product:{id}";
                string? cachedProduct = await _distributedCache.GetStringAsync(cacheKey);

                if (cachedProduct != null)
                {
                    ProductDto? productFromCache=
                    JsonSerializer.Deserialize<ProductDto>(cachedProduct);
                    return productFromCache;
                }


                HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/product-id/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        ProductDto? productFromFallbackPolicy = await response.Content.ReadFromJsonAsync<ProductDto>();
                        if (productFromFallbackPolicy == null)
                        {
                            throw new NotImplementedException("Fallback policy was not implemented");
                        }

                        return productFromFallbackPolicy;

                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("Bad Request", null, System.Net.HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                    }
                }

                ProductDto? product = await response.Content.ReadFromJsonAsync<ProductDto>();
                if (product == null)
                {
                    throw new ArgumentException("Product could not be found");
                }

                //Key: product:123
                // Value: {"PRoductName:"...",..."}
                string productJson = JsonSerializer.Serialize(product);

                DistributedCacheEntryOptions options=
                    new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10));

                string cachedKeyToWrite = $"product:{id}";

                await _distributedCache.SetStringAsync(cachedKeyToWrite, productJson, options);

                return product;
            }
            catch (BulkheadRejectedException ex)
            //catch (HttpRequestException ex)
            {
                // Handle the exception as needed
                //throw new HttpRequestException("An error occurred while fetching the product", ex);

                _logger.LogError(ex, "Bulkhead isolation triggered: The request was rejected due to too many concurrent requests");

                return new ProductDto(
                    ProductID: Guid.Empty,
                    ProductName: "Temporarily Unavailable",
                    Category: "Temporarily Unavailable",
                    UnitPrice: 0,
                    QuantityInStock: 0);
            }
            

        }
    }
}
