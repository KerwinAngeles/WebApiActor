using Microsoft.EntityFrameworkCore;
using WebApiActor.Data;
using WebApiActor.Services;
using WebApiActor.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("AppConnection");
builder.Services.AddDbContext<ApplicationDbContext>(op => op.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IOrdenarActores, OrdenarActoresService>();

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
