using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Misc
{
  public class ModelMethods
  {
    public static List<OrderSummary> ComputeBillSummary(Bill bill)
    {
      var summary = new List<OrderSummary>();

      foreach (Order order in bill.Orders)
      {
        var ordSummary = summary.FirstOrDefault(o => o.ProductName == order.ProductName);
        if (ordSummary == null)
        {
          summary.Add(new OrderSummary()
          {
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            TotalPrice = order.PricePerUnit * order.Quantity
          });
        }
        else
        {
          ordSummary.Quantity += order.Quantity;
          ordSummary.TotalPrice += order.Quantity * order.PricePerUnit;
        }
      }

      return summary;
    }

    public static int GetCurrentlyConsumedQuantity(Customer customer)
    {
      return customer.GetCurrentBill().Orders.Aggregate(0, (prod, next) => next.Quantity + prod);
    }
  }

  public class OrderSummary
  {
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
  }
}