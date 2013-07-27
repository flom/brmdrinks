using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL
{
  //public class DrinksInitializer : DropCreateDatabaseIfModelChanges<DrinksContext>
  public class DrinksInitializer : DropCreateDatabaseAlways<DrinksContext>
  {
    protected override void Seed(DrinksContext context)
    {
      var drink1 = new Product() { Name = "Fassbier (0.4L)", Price = 0.9M };
      var drink2 = new Product() { Name = "Weizenbier (0.5L)", Price = 1.2M };
      var drink3 = new Product() { Name = "Wasser (1.0L)", Price = 0.7M };
      var drink4 = new Product() { Name = "Africola (1.0L)", Price = 1.1M };
      var drink5 = new Product() { Name = "Karamalz (0.33L)", Price = 0.8M };
      var drink6 = new Product() { Name = "Limo (1.0L)", Price = 1.0M };
      context.Products.Add(drink1);
      context.Products.Add(drink2);
      context.Products.Add(drink3);
      context.Products.Add(drink4);
      context.Products.Add(drink5);
      context.Products.Add(drink6);
      context.SaveChanges();

      var acc1 = new Account() { Name = "Jerry Murray" };
      var acc2 = new Account() { Name = "Kieran Clements" };
      var acc3 = new Account() { Name = "Keegan Deleon" };
      context.Accounts.Add(acc1);
      context.Accounts.Add(acc2);
      context.Accounts.Add(acc3);
      context.SaveChanges();

      var cust1 = new Customer() { FirstName = "Jerry", LastName = "Murray", Account = acc1 };
      var cust2 = new Customer() { FirstName = "Kieran", LastName = "Clements", Account = acc2 };
      var cust3 = new Customer() { FirstName = "Ulric", LastName = "Faulkner", Account = acc2 };
      var cust4 = new Customer() { LastName = "Yuli Kemp", Account = acc3 };
      var cust5 = new Customer() { LastName = "Keegan Deleon", Account = acc3 };
      context.Customers.Add(cust1);
      context.Customers.Add(cust2);
      context.Customers.Add(cust3);
      context.Customers.Add(cust4);
      context.Customers.Add(cust5);
      context.SaveChanges();

      var bill1 = new Bill() { From = new DateTime(2013, 2, 1), Customer = cust1 };
      var bill2 = new Bill() { From = new DateTime(2013, 1, 1), To = new DateTime(2013, 1, 31), Customer = cust2 };
      var bill3 = new Bill() { From = new DateTime(2013, 2, 1), Customer = cust2 };
      var bill4 = new Bill() { From = new DateTime(2013, 2, 1), Customer = cust3 };
      var bill5 = new Bill() { From = new DateTime(2013, 2, 1), Customer = cust4 };
      var bill6 = new Bill() { From = new DateTime(2013, 2, 1), Customer = cust5 };
      context.Bills.Add(bill1);
      context.Bills.Add(bill2);
      context.Bills.Add(bill3);
      context.Bills.Add(bill4);
      context.Bills.Add(bill5);
      context.Bills.Add(bill6);
      context.SaveChanges();

      var order1 = new Order() { PurchaseDate = new DateTime(2013, 2, 1), ProductName = "Fassbier (0.4L)", PricePerUnit = 0.9M, Quantity = 1, Bill = bill1 };
      for (int i = 1; i < 10; i++)
      {
        context.Orders.Add(new Order() { PurchaseDate = DateTime.Now.AddHours(i * -1), ProductName = "Fassbier (0.4L)", PricePerUnit = 0.9M, Quantity = i, Bill = bill1 });
        context.Orders.Add(new Order() { PurchaseDate = DateTime.Now.AddHours(i * -1), ProductName = "Wasser (1.0L)", PricePerUnit = 0.6M, Quantity = i, Bill = bill1 });
        context.Orders.Add(new Order() { PurchaseDate = DateTime.Now.AddHours(i * -1), ProductName = "Weizenbier (0.4L)", PricePerUnit = 1.2M, Quantity = i, Bill = bill2 });
        context.Orders.Add(new Order() { PurchaseDate = DateTime.Now.AddHours(i * -1), ProductName = "Fassbier (0.4L)", PricePerUnit = 0.9M, Quantity = i, Bill = bill3 });
        context.Orders.Add(new Order() { PurchaseDate = DateTime.Now.AddHours(i * -1), ProductName = "Africola (1.0L)", PricePerUnit = 0.8M, Quantity = i, Bill = bill4 });
      }
      context.SaveChanges();
    }
  }
}