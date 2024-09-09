using ManagmentSystem.Application;
using ManagmentSystem.DataAccess;
using ManagmentSystem.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using ManagmentSystem.Application.Abstractions.Data;
using Microsoft.OpenApi.Models;
using ManagementSystem.Extensions;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.Application.Abstractions.Auth;
using ManagmentSystem.Application.Auth;
using Microsoft.AspNetCore.CookiePolicy;
using ManagementSystem;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.SchemaFilter<EnumSchemeFilter>();
});

builder.Services.AddDbContext<ManagmentSystemDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ManagmentSystem.Application.AssemblyReference.Assembly));

builder
    .Services
    .AddControllers()
    .AddApplicationPart(ManagmentSystem.Presentation.AssemblyReference.Assembly);

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddApiAuthentication(configuration.GetSection(nameof(JwtOptions)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{ 
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
