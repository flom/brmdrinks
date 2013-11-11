using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IOptionsRepository : IRepository<Options>
  {
    int? GetPriorityProduct();
  }

  public class OptionsRepository : BaseRepository<Options>, IOptionsRepository
  {
    public int? GetPriorityProduct()
    {
      foreach (var opt in GetAll())
      {
        return opt.PriorityProductId;
      }
      return null;
    }
  }
}