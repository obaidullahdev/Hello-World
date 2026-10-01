using HelloApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GreetingService>();
builder.Services.AddSingleton<VersionService>();
builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => "Hello World!");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/version", (VersionService versions) => Results.Ok(new { version = versions.GetVersion() }));
app.MapGet("/greet/{name}", (string name, GreetingService greetings) =>
    greetings.TryGreet(name, out var message)
        ? Results.Ok(new { message })
        : Results.Problem(
            title: "Invalid name",
            detail: greetings.Validate(name),
            statusCode: StatusCodes.Status400BadRequest));

app.Run();

public partial class Program;
