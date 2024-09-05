using ManagmentSystem.Application.Services;
using ManagmentSystem.Application;
using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.DataAccess;
using ManagmentSystem.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ManagmentSystemDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ManagmentSystem.Application.AssemblyReference.Assembly));

builder
    .Services
    .AddControllers()
    .AddApplicationPart(ManagmentSystem.Presentation.AssemblyReference.Assembly);

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
