using BookAPIService.Clients;
using BookAPIService.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<IOpenLibraryClient, OpenLibraryClient>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org.invalid/");
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler((sp, _) =>
{
    var logger = sp.GetRequiredService<ILogger<OpenLibraryClient>>();
    return PollyPolicies.GetCircuitBreakerPolicy(logger);
})
.AddPolicyHandler((sp, _) =>
{
    var logger = sp.GetRequiredService<ILogger<OpenLibraryClient>>();
    return PollyPolicies.GetRetryPolicy(logger);
})
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));


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
