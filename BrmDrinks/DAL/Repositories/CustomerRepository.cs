using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface ICustomerRepository : IRepository<Customer>
  {
  }

  public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
  {
    public CustomerRepository() : base() { }
    public CustomerRepository(DrinksContext context) : base(context) { }
  }
}