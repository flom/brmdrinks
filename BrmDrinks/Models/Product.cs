using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Product : Base
  {
    public string Name { get; set; }
    [Display(Name="Preis in €")]
    public decimal Price { get; set; }
  }
}