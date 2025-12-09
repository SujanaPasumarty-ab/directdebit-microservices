var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 🔸 TEMP: comment this out to avoid any redirect confusion in App Service
// app.UseHttpsRedirection();

app.UseAuthorization();

// ✅ Simple "it works" endpoint at root
app.MapGet("/", () => Results.Text("Cancellation Subscription Service is running", "text/plain"));

// ✅ Health check endpoint for App Service probes
app.MapHealthChecks("/health");

// Map your API controllers
app.MapControllers();

// Log a message when the app has started successfully.
app.Lifetime.ApplicationStarted.Register(() =>
{
    app.Logger.LogInformation("Cancellation microservice is running successfully");
});

app.Run();
