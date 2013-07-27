using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Bill : Base
  {
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual ICollection<Order> Orders { get; set; }

    public Bill()
    {
      Orders = new List<Order>();
    }
  }
}