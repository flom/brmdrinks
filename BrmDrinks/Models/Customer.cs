using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Customer : Base
  {
    [Display(Name = "Vorname")]
    public string FirstName { get; set; }
    [Display(Name = "Nachname")]
    public string LastName { get; set; }
    public bool Archived { get; set; }

    public virtual Account Account { get; set; }
    public virtual ICollection<Bill> Bills { get; set; }

    public Customer()
    {
      FirstName = "";
      LastName = "";
      Bills = new List<Bill>();
    }

    public string GetFullName()
    {
      if (string.IsNullOrEmpty(FirstName))
        return LastName;
      else if (string.IsNullOrEmpty(LastName))
        return FirstName;
      else
        return string.Format("{1}, {0}", FirstName, LastName);
    }

    public Bill GetCurrentBill()
    {
      return Bills.First(b => b.To == null);
    }
  }
}