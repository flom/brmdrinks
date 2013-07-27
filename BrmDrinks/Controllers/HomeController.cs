using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrmDrinks.Controllers
{
  public class HomeController : Controller
  {
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
  }
}
