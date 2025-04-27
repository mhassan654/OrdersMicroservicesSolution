using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace BusinessLogicLayer.Policies;

public class PollyPolicies:IPollyPolicies
{
    private readonly ILogger<PollyPolicies> _logger;

    public PollyPolicies(ILogger<PollyPolicies> logger)
    {
        _logger = logger;
    }
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        AsyncRetryPolicy<HttpResponseMessage>
            policy = Policy.HandleResult<HttpResponseMessage>(
                    r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: retryCount, // number of retries
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (response, timespan, retryAttempt, context) =>
                    {
                        // add logs:
                        _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                    });

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
        int handledEventsAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage>
            policy = Policy.HandleResult<HttpResponseMessage>(
                    r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking, // number of retries
                    durationOfBreak: durationOfBreak, // Delay between
                    onBreak: (outcome, timespan) =>
                    {
                        // add logs:
                        _logger.LogInformation(
                            $"Circuit breaker opened for {timespan.TotalMinutes} minutes due to consecutive 3" +
                            $"failures. The subsequent requests will be blocked");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset. The subsequent requests will be allowed");
                    });

        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeOut)
    {
        AsyncTimeoutPolicy<HttpResponseMessage>
            policy = Policy.TimeoutAsync<HttpResponseMessage>
                (timeOut);
               
        return policy;
    }

  
}