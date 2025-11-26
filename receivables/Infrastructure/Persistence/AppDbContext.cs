using Microsoft.EntityFrameworkCore;
using receivables.Infrastructure.Entities;

namespace Receivables.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Cnpj).IsRequired().HasMaxLength(14);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.MonthlyRevenue).HasPrecision(18,2);
            e.Property(c => c.CreditLimit).HasPrecision(18,2);
            e.Property(c => c.Segment).HasConversion<string>();
            e.HasMany<Invoice>().WithOne().HasForeignKey(i => i.CompanyId);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Number).IsRequired().HasMaxLength(50);
            e.Property(i => i.Amount).HasPrecision(18,2);
            e.Property(i => i.DueDate).IsRequired();
        });
    }
}
