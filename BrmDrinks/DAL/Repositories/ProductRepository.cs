using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IProductRepository : IRepository<Product>
  {
  }

  public class ProductRepository : BaseRepository<Product>, IProductRepository
  {
    public ProductRepository() : base() { }
    public ProductRepository(DrinksContext context) : base(context) { }
  }
}