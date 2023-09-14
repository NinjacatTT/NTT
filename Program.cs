var builder = WebApplication.CreateBuilder(args);

// use port 80 and 443 if not in development
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:80", "https://*:443");
}

var app = builder.Build();

app.MapGet("/", () => "Hello World");

app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();
