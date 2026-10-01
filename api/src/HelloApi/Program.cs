using HelloApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GreetingService>();
builder.Services.AddSingleton<VersionService>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/ping", () => Results.Ok(new { pong = true }));
app.MapGet("/version", (VersionService versions) => Results.Ok(new { version = versions.GetVersion() }));
app.MapGet("/greet/{name}", (string name, GreetingService greetings) =>
    greetings.TryGreet(name, out var message)
        ? Results.Ok(new { message })
        : Results.BadRequest(new { error = "Name must not be blank." }));

app.Run();

public partial class Program;
