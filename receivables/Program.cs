using Microsoft.EntityFrameworkCore;
using Receivables.Domain.Services;
using Receivables.Infrastructure.Persistence;
using Receivables.Infrastructure.Repositories;
using receivables.Infrastructure.Repositories;
using receivables.Domain.Services;
using receivables.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=ReceivablesDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<ICreditLimitCalculator, CreditLimitCalculator>();
builder.Services.AddSingleton<AnticipationCalculator>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

builder.Services.AddScoped<ICompaniesService, CompaniesService>();
builder.Services.AddScoped<IInvoicesService, InvoicesService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

// DTO inputs
public record CompanyInput(string Cnpj, string Name, decimal MonthlyRevenue, string Segment);
public record InvoiceInput(string Number, decimal Amount, DateTime DueDate);
