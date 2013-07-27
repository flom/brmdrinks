using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface ISettlementRepository : IRepository<Settlement>
  {
  }

  public class SettlementRepository : BaseRepository<Settlement>, ISettlementRepository
  {
    public SettlementRepository() : base() { }
    public SettlementRepository(DrinksContext context) : base(context) { }
  }
}