
using Polly;
using Polly.Extensions.Http;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger, ResilienceOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning(
                    "Polly Retry {RetryAttempt} after {Delay}ms",
                    retryAttempt,
                    timespan.TotalMilliseconds);
            });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger, ResilienceOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.CircuitBreakerFailures,
                durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds),
                onBreak: (outcome, breakDelay) =>
            {
                logger.LogError(
                    "Circuit breaker OPEN for {BreakDelay}s",
                    breakDelay.TotalSeconds);
            },

            onReset: () =>
            {
                logger.LogInformation(
                    "Circuit breaker RESET (closed). External service is healthy.");
            },

            onHalfOpen: () =>
            {
                logger.LogWarning(
                    "Circuit breaker HALF-OPEN. Next call is a trial request.");
            }
            );
    }

}