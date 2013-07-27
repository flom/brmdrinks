using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Account : Base
  {
    public string Name { get; set; }

    public virtual ICollection<Customer> Customers { get; set; }

    public Account()
    {
      Customers = new List<Customer>();
    }
  }
}