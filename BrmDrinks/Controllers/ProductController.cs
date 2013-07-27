using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrmDrinks.Models;
using BrmDrinks.DAL;
using BrmDrinks.DAL.Repositories;

namespace BrmDrinks.Controllers
{
  public class ProductController : Controller
  {
    private IProductRepository Products;

    public ProductController() : this(new ProductRepository()) { }

    public ProductController(IProductRepository products)
    {
      Products = products;
    }

    //
    // GET: /Product/

    public ActionResult Index()
    {
      return View(Products.GetAll());
    }

    //
    // GET: /Product/Details/5

    public ActionResult Details(int id = 0)
    {
      Product product = Products.GetById(id);
      if (product == null)
      {
        return HttpNotFound();
      }
      return View(product);
    }

    //
    // GET: /Product/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Product/Create

    [HttpPost]
    public ActionResult Create(Product product)
    {
      if (ModelState.IsValid)
      {
        Products.Create(product);
        return RedirectToAction("Index");
      }

      return View(product);
    }

    //
    // GET: /Product/Edit/5

    public ActionResult Edit(int id = 0)
    {
      Product product = Products.GetById(id);
      if (product == null)
      {
        return HttpNotFound();
      }
      return View(product);
    }

    //
    // POST: /Product/Edit/5

    [HttpPost]
    public ActionResult Edit(Product product)
    {
      if (ModelState.IsValid)
      {
        Products.Update(product);
        return RedirectToAction("Index");
      }
      return View(product);
    }

    //
    // GET: /Product/Delete/5

    public ActionResult Delete(int id = 0)
    {
      Product product = Products.GetById(id);
      if (product == null)
      {
        return HttpNotFound();
      }
      return View(product);
    }

    //
    // POST: /Product/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Products.Delete(id);
      return RedirectToAction("Index");
    }

  }
}