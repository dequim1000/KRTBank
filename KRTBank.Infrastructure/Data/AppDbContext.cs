using KRTBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("Accounts");
        modelBuilder.Entity<Account>().HasKey(a => a.Id);
        modelBuilder.Entity<Account>().Property(a => a.HolderName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Account>().Property(a => a.CPF).IsRequired().HasMaxLength(11);
        modelBuilder.Entity<Account>().Property(a => a.Status).IsRequired();
    }
}