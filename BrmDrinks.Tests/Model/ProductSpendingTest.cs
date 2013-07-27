using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrmDrinks.Models;

namespace BrmDrinks.Tests.Model
{
  [TestClass]
  public class ProductSpendingTest
  {
    [TestMethod]
    public void TestQuotaExhausted()
    {
      var spend = new ProductSpending();
      spend.MaxQuantity = 20;

      Assert.AreEqual(false, spend.QuotaExhausted());
      spend.Orders.Add(new Order() { Quantity = 5 });
      Assert.AreEqual(false, spend.QuotaExhausted());
      spend.Orders.Add(new Order() { Quantity = 14 });
      Assert.AreEqual(false, spend.QuotaExhausted());
      spend.Orders.Add(new Order() { Quantity = 1 });
      Assert.AreEqual(true, spend.QuotaExhausted());
      spend.Orders.Add(new Order() { Quantity = 5 });
      Assert.AreEqual(true, spend.QuotaExhausted());
    }
  }
}
