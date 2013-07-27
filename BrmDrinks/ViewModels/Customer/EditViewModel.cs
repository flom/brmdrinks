using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrmDrinks.ViewModels.Customer
{
  public class EditViewModel
  {
    public int CustomerId { get; set; }
    [Display(Name = "Vorname")]
    public string FirstName { get; set; }
    [Display(Name = "Nachname")]
    public string LastName { get; set; }
    public int AccountId { get; set; }
    public string NewAccountName { get; set; }
    [Display(Name = "Archiviert?")]
    public bool Archived { get; set; }

    public List<Account> AvailableAccounts { get; set; }

    public EditViewModel()
    {
    }

    public EditViewModel(Models.Customer customer, List<Account> accounts)
    {
      CustomerId = customer.ID;
      FirstName = customer.FirstName;
      LastName = customer.LastName;
      if (customer.Account != null)
      {
        AccountId = customer.Account.ID;
      }

      AvailableAccounts = accounts;
    }
  }
}