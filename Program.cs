using Microsoft.OpenApi.Models;
using UserManagementAPI.Middleware;
using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagementAPI", Version = "v1" });
});

// Register repository
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

var app = builder.Build();

// Middleware order: Exception -> Auth -> Logging
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SimpleTokenAuthMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagementAPI v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
