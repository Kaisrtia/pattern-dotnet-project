using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Repositories;
using pattern_project.Repositories.Interfaces;
using pattern_project.Services;
using pattern_project.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.Cookie.Name = "medical-record-auth";
      options.Cookie.HttpOnly = true;
      options.SlidingExpiration = true;
      options.Events.OnRedirectToLogin = context =>
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
      };
      options.Events.OnRedirectToAccessDenied = context =>
      {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
      };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<pattern_project.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();