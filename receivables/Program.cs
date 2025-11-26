using Microsoft.EntityFrameworkCore;
using Receivables.Domain.Services;
using Receivables.Infrastructure.Persistence;
using Receivables.Infrastructure.Repositories;
using receivables.Infrastructure.Repositories;
using receivables.Domain.Services;
using receivables.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "Receivables API",
        Description = "API para gerenciamento de recebíveis e antecipação de notas fiscais",
        Contact = new()
        {
            Name = "Receivables Team"
        }
    });
    
    c.EnableAnnotations();
});

// Configuração do DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=ReceivablesDb;Trusted_Connection=True;MultipleActiveResultSets=true"),
    ServiceLifetime.Scoped);

// Registro de serviços de domínio
builder.Services.AddSingleton<ICreditLimitCalculator, CreditLimitCalculator>();
builder.Services.AddSingleton<AnticipationCalculator>();

// Registro de repositórios
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Registro de serviços de aplicação
builder.Services.AddScoped<ICompaniesService, CompaniesService>();
builder.Services.AddScoped<IInvoicesService, InvoicesService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configurar o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
