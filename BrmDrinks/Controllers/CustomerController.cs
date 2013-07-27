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
using BrmDrinks.Misc;
using BrmDrinks.ViewModels.Customer;

namespace BrmDrinks.Controllers
{
  public class CustomerController : Controller
  {
    private ICustomerRepository Customers;
    private IBillRepository Bills;

    public CustomerController() : this(new DrinksContext()) { }

    public CustomerController(DrinksContext context) : this(new CustomerRepository(context), new BillRepository(context)) { }

    public CustomerController(ICustomerRepository customers, IBillRepository bills)
    {
      Customers = customers;
      Bills = bills;
    }

    //
    // GET: /Customer/

    public ActionResult Index()
    {
      return View(Customers.GetAll());
    }

    //
    // GET: /Customer/Details/5

    public ActionResult Details(int id = 0)
    {
      Customer customer = Customers.GetById(id);
      if (customer == null)
      {
        return HttpNotFound();
      }
      return View(customer);
    }

    //
    // GET: /Customer/Create

    public ActionResult Create()
    {
      var viewmodel = new EditViewModel(new Customer(), Customers.Context.Accounts.ToList());
      return View("Edit", viewmodel);
    }

    [HttpPost]
    public ActionResult Create(EditViewModel viewmodel)
    {
      var customer = new Customer()
      {
        FirstName = viewmodel.FirstName,
        LastName = viewmodel.LastName,
        Archived = viewmodel.Archived
      };
      customer.Bills.Add(new Bill() { From = DateTime.Now, Customer = customer });
      customer.Account = !String.IsNullOrEmpty(viewmodel.NewAccountName) ? new Account() { Name = viewmodel.NewAccountName } : Customers.Context.Accounts.Find(viewmodel.AccountId);
      Customers.Create(customer);

      return RedirectToAction("Index");
    }

    //
    // GET: /Customer/Edit/5

    public ActionResult Edit(int id = 0)
    {
      Customer customer = Customers.GetById(id);
      if (customer == null)
      {
        return HttpNotFound();
      }
      var viewmodel = new EditViewModel(customer, Customers.Context.Accounts.ToList());
      return View(viewmodel);
    }

    //
    // POST: /Customer/Edit/5

    [HttpPost]
    public ActionResult Edit(EditViewModel viewmodel)
    {
      if (ModelState.IsValid)
      {
        Customer customer = Customers.GetById(viewmodel.CustomerId);
        customer.FirstName = viewmodel.FirstName;
        customer.LastName = viewmodel.LastName;
        customer.Archived = viewmodel.Archived;
        customer.Account = String.IsNullOrEmpty(viewmodel.NewAccountName) ? new Account() { Name = viewmodel.NewAccountName } : Customers.Context.Accounts.Find(viewmodel.AccountId);
        Customers.Context.Accounts.Add(customer.Account);
        Customers.Update(customer);
        return RedirectToAction("Index");
      }
      return View(viewmodel);
    }

    //
    // GET: /Customer/Delete/5

    public ActionResult Delete(int id = 0)
    {
      Customer customer = Customers.GetById(id);
      if (customer == null)
      {
        return HttpNotFound();
      }
      return View(customer);
    }

    //
    // POST: /Customer/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var customer = Customers.GetById(id);
      int totalOrders = 0;
      foreach (Bill bill in customer.Bills)
      {
        totalOrders += bill.Orders.Count;
      }
      if (totalOrders == 0)
      {
        customer.Bills.ToList().ForEach(bill => Customers.Context.Bills.Remove(bill));
        Customers.Delete(id);
      }

      return RedirectToAction("Index");
    }

    public ActionResult ToggleArchive(int id)
    {
      Customer customer = Customers.GetById(id);
      customer.Archived = !customer.Archived;
      Customers.Update(customer);

      return RedirectToAction("Index", "Home");
    }
  }
}