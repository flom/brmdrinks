using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL
{
  public class DrinksContext : DbContext
  {
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Settlement> Settlements { get; set; }
    public DbSet<ProductSpending> ProductSpendings { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ProductSpending>().HasMany(p => p.Orders).WithOptional().WillCascadeOnDelete(true);
    }
  }
}