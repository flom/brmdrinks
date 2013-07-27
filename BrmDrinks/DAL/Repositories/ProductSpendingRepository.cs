using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IProductSpendingRepository : IRepository<ProductSpending>
  {
  }

  public class ProductSpendingRepository : BaseRepository<ProductSpending>, IProductSpendingRepository
  {
    public ProductSpendingRepository() : base() { }
    public ProductSpendingRepository(DrinksContext context) : base(context) { }
  }
}