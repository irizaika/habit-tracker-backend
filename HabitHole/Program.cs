using HabbitHole.Data;
using HabitHole.Services;
using HabitHole.Services.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SQLitePCL;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

Batteries.Init();

// Add services to the container.
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<Mapper, Mapper>();
builder.Services.AddScoped<IHabitEntryService, HabitEntryService>();
builder.Services.AddScoped<IHabitSummaryService, HabitSummaryService>();
builder.Services.AddScoped<IDateProvider, DateProvider>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "HabitHole API";
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowed = new[]
        {
            "http://localhost:5173",
        };

        policy.WithOrigins(allowed.Where(x => !string.IsNullOrEmpty(x)).ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API");
        c.RoutePrefix = string.Empty;
    });

    app.UseOpenApi();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowFrontend");

ApplyMigration();

app.Run();


void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (_db.Database.GetPendingMigrations().Any())
    {
        _db.Database.Migrate();
    }
}