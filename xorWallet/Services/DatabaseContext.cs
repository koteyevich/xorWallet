using Microsoft.EntityFrameworkCore;
using xorWallet.Models;

namespace xorWallet.Services;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<InvoiceModel> Invoices { get; set; }
    public DbSet<CheckModel> Checks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>();

        modelBuilder.Entity<InvoiceModel>();
        modelBuilder.Entity<CheckModel>();
    }
}