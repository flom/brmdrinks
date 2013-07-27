using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrmDrinks.Models;
using BrmDrinks.Misc;
using System.Collections.Generic;

namespace BrmDrinks.Tests.Misc
{
  [TestClass]
  public class ModelMethodsTest
  {
    [TestMethod]
    public void TestComputeBillSummary()
    {
      var bill = new Bill();
      var order1 = new Order() { ProductName = "foo", PricePerUnit = 1.5M, Quantity = 2, Bill = bill };
      var order2 = new Order() { ProductName = "foo", PricePerUnit = 1.5M, Quantity = 3, Bill = bill };
      var order3 = new Order() { ProductName = "bar", PricePerUnit = 2M, Quantity = 1, Bill = bill };
      var order4 = new Order() { ProductName = "baz", PricePerUnit = 0.8M, Quantity = 6, Bill = bill };
      bill.Orders.Add(order1);
      bill.Orders.Add(order2);
      bill.Orders.Add(order3);
      bill.Orders.Add(order4);

      List<OrderSummary> summary = ModelMethods.ComputeBillSummary(bill);

      Assert.AreEqual(3, summary.Count);
      Assert.AreEqual(5 * 1.5M, summary.Find(s => s.ProductName == "foo").TotalPrice);
      Assert.AreEqual(2M, summary.Find(s => s.ProductName == "bar").TotalPrice);
      Assert.AreEqual(6 * 0.8M, summary.Find(s => s.ProductName == "baz").TotalPrice);
    }
  }
}
