using Microsoft.EntityFrameworkCore;
using RoadWorkClub.API.Data;
using RoadWorkClub.API.Interfaces;
using RoadWorkClub.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RoadWorkClubDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RoadWalksConnectionString")));
builder.Services.AddScoped<IPathwayRepository, PathwayRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
