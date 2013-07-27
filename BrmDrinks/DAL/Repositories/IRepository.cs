using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public interface IRepository<Entity>
  {
    IEnumerable<Entity> GetAll();
    Entity GetById(int id);
    void Create(Entity entity);
    void Update(Entity entity);
    void Delete(Entity entity);
    void Delete(int id);
    void SaveChanges();

    DrinksContext Context { get; set; }
  }
}