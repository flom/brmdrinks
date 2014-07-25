using BrmDrinks.DAL.Repositories;
using BrmDrinks.Models;
using BrmDrinks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrmDrinks.Controllers
{
  public class HomeController : Controller
  {
    private IProductRepository Products;
    private IOptionsRepository OptionsRepo;

    public HomeController()
      : this(new ProductRepository(), new OptionsRepository())
    {

    }

    public HomeController(IProductRepository products, IOptionsRepository options)
    {
      Products = products;
      OptionsRepo = options;
    }

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult IndexOld()
    {
      return View();
    }

    public ActionResult About()
    {
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }

    public ActionResult DrinkAdmin()
    {
      return View();
    }

    public ActionResult OldSettlements()
    {
      return View();
    }

    public ActionResult ShowSettlement(int id)
    {
      ViewBag.SettlementId = id;
      return View();
    }

    public ActionResult Statistics()
    {
      return View();
    }

    public ActionResult Options()
    {
      var viewmodel = new OptionsViewModel()
      {
        Products = Products.GetAll(),
        PriorityProductId = OptionsRepo.GetPriorityProduct()
      };

      return View(viewmodel);
    }

    [HttpPost]
    public ActionResult Options(int productId = 0)
    {
      if (productId != 0)
      {
        var opt = OptionsRepo.GetAll().FirstOrDefault();
        if (opt != null)
        {
          opt.PriorityProductId = productId;
          OptionsRepo.Update(opt);
        }
        else
        {
          OptionsRepo.Create(new Options() { PriorityProductId = productId });
        }
      }
      return RedirectToAction("Options");
    }

    public ActionResult PersonStatistic()
    {
      return View();
    }
  }
}
