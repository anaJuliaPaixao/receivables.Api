using Microsoft.EntityFrameworkCore;
using Receivables.Domain.Services;
using Receivables.Infrastructure.Persistence;
using Receivables.Infrastructure.Repositories;
using receivables.Infrastructure.Repositories;
using receivables.Domain.Services;
using receivables.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "Receivables API",
        Description = "API para gerenciamento de receb�veis e antecipa��o de notas fiscais",
        Contact = new()
        {
            Name = "Receivables Team"
        }
    });
    
    c.EnableAnnotations();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=receivables_db;Username=receivables;Password=receivables123"),
    ServiceLifetime.Scoped);

builder.Services.AddSingleton<ICreditLimitCalculator, CreditLimitCalculator>();
builder.Services.AddSingleton<AnticipationCalculator>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<ICompaniesService, CompaniesService>();
builder.Services.AddScoped<IInvoicesService, InvoicesService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Receivables API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();