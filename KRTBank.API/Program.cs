using KRTBank.Application.Services;
using KRTBank.Domain.Events;
using KRTBank.Domain.Interfaces;
using KRTBank.Infrastructure.Data;
using KRTBank.Infrastructure.NotificationHandlers;
using KRTBank.Infrastructure.Repositories;
using KRTBank.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AccountCreatedEvent).Assembly));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<FraudPreventionService>();
builder.Services.AddScoped<CreditCardService>();
builder.Services.AddScoped<FraudPreventionHandler>();
builder.Services.AddScoped<CreditCardHandler>();

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