using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class ProductSpending : Base
  {
    public int MaxQuantity { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
    public virtual Product Product { get; set; }
    public virtual Customer Customer { get; set; }

    public ProductSpending()
    {
      Orders = new List<Order>();
    }

    public bool QuotaExhausted()
    {
      return GetAlreadyConsumed() >= MaxQuantity;
    }

    public int GetAlreadyConsumed()
    {
      return Orders.Aggregate(0, (sum, next) => sum + next.Quantity);
    }
  }
}