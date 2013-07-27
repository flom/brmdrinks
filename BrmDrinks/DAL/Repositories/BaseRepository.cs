using BrmDrinks.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BrmDrinks.DAL.Repositories
{
  public class BaseRepository<Entity> : IRepository<Entity> where Entity : Base
  {
    public DrinksContext Context { get; set; }
    public DbSet<Entity> Entities { get; set; }

    public BaseRepository() : this(new DrinksContext()) { }

    public BaseRepository(DrinksContext context)
    {
      Context = context;
      Entities = Context.Set<Entity>();
    }

    public IEnumerable<Entity> GetAll()
    {
      return Entities;
    }

    public Entity GetById(int id)
    {
      return Entities.Find(id);
    }

    public void Create(Entity entity)
    {
      Entities.Add(entity);
      SaveChanges();
    }

    public void Update(Entity entity)
    {
      Context.Entry(entity).State = System.Data.EntityState.Modified;
      SaveChanges();
    }

    public void Delete(Entity entity)
    {
      Entities.Remove(entity);
      SaveChanges();
    }

    public void Delete(int id)
    {
      Delete(GetById(id));
    }

    public void SaveChanges()
    {
      Context.SaveChanges();
    }
  }
}