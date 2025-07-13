using Microsoft.EntityFrameworkCore;
using Server_Library.Data;
using Server_Library.Helpers;
using Server_Library.Repositories.Contracts;
using Server_Library.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("Your connection string is wrong") ));
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();












// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
