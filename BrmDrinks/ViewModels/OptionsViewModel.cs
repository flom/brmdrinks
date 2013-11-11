using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.ViewModels
{
  public class OptionsViewModel
  {
    public IEnumerable<Product> Products { get; set; }
    public int? PriorityProductId { get; set; }
  }
}