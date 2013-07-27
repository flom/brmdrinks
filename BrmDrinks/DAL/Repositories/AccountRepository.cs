using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IAccountRepository : IRepository<Account>
  {
  }

  public class AccountRepository : BaseRepository<Account>, IAccountRepository
  {
    public AccountRepository() : base() { }
    public AccountRepository(DrinksContext context) : base(context) { }
  }
}