using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IBillRepository : IRepository<Bill>
  {
  }

  public class BillRepository : BaseRepository<Bill>, IBillRepository
  {
    public BillRepository() : base() { }
    public BillRepository(DrinksContext context) : base(context) { }
  }
}