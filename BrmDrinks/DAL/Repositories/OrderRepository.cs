using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IOrderRepository : IRepository<Order>
  {
  }

  public class OrderRepository : BaseRepository<Order>, IOrderRepository
  {
    public OrderRepository() : base() { }
    public OrderRepository(DrinksContext context) : base(context) { }
  }
}