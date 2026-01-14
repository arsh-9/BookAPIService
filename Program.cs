using BookAPIService.Clients;
using BookAPIService.Services;
using Microsoft.Extensions.Options;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<OpenLibraryOptions>(
    builder.Configuration.GetSection("OpenLibrary"));

builder.Services.Configure<ResilienceOptions>(
    builder.Configuration.GetSection("Resilience"));

builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection("Cache"));

builder.Services.AddHttpClient<IOpenLibraryClient, OpenLibraryClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<OpenLibraryOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

})
.AddPolicyHandler((sp, _) =>
{
    var logger = sp.GetRequiredService<ILogger<OpenLibraryClient>>();
    var resilience = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;
    return PollyPolicies.GetCircuitBreakerPolicy(logger, resilience);
})
.AddPolicyHandler((sp, _) =>
{
    var logger = sp.GetRequiredService<ILogger<OpenLibraryClient>>();
    var resilience = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;
    return PollyPolicies.GetRetryPolicy(logger, resilience);
})
.AddPolicyHandler((sp, _) =>
{
    var resilience = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;
    return Policy.TimeoutAsync<HttpResponseMessage>(
        TimeSpan.FromSeconds(resilience.TimeoutSeconds));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
