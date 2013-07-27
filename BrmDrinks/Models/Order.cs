using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Order : Base
  {
    public DateTime PurchaseDate { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }

    public virtual Bill Bill { get; set; }
  }
}