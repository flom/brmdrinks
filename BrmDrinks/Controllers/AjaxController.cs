using BrmDrinks.DAL;
using BrmDrinks.DAL.Repositories;
using BrmDrinks.Misc;
using BrmDrinks.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace BrmDrinks.Controllers
{
  public class AjaxController : Controller
  {
    private ICustomerRepository Customers;
    private IProductRepository Products;
    private IOrderRepository Orders;
    private IAccountRepository Accounts;
    private ISettlementRepository Settlements;
    private IProductSpendingRepository ProductSpendings;
    private IOptionsRepository Options;

    public AjaxController() : this(new DrinksContext()) { }

    public AjaxController(DrinksContext context)
      : this(new CustomerRepository(context), new ProductRepository(context), new OrderRepository(context), new AccountRepository(context),
        new SettlementRepository(context), new ProductSpendingRepository(context), new OptionsRepository()) { }

    public AjaxController(ICustomerRepository customers, IProductRepository products, IOrderRepository orders, IAccountRepository accounts,
      ISettlementRepository settlements, IProductSpendingRepository productSpendings, IOptionsRepository options)
    {
      Customers = customers;
      Products = products;
      Orders = orders;
      Accounts = accounts;
      Settlements = settlements;
      ProductSpendings = productSpendings;
      Options = options;
    }

    public JsonResult GetAllProducts()
    {
      var consumption = new Dictionary<string, int>();
      foreach (Customer customer in Customers.GetAll())
      {
        foreach (Order order in customer.GetCurrentBill().Orders)
        {
          if (consumption.ContainsKey(order.ProductName))
            consumption[order.ProductName] += order.Quantity;
          else
            consumption[order.ProductName] = order.Quantity;
        }
      }

      int priorityId = Options.GetPriorityProduct() ?? 0;
      var products = from element in Products.GetAll().ToList()
                     orderby element.Name
                     select new
                     {
                       id = element.ID,
                       name = element.Name,
                       price = element.Price,
                       consumed = consumption.ContainsKey(element.Name) ? consumption[element.Name] : 0,
                       priority = element.ID == priorityId,
                     };

      return Json(products);
    }

    /**
     * sortOption:
     *  0 - alphabetical sort
     *  1 - sort after consumed products
     * */
    public JsonResult GetAllCustomers(int sortOption = 0)
    {
      if (sortOption == 1)
      {
        var customers = from element in Customers.GetAll().ToList()
                        where !element.Archived
                        orderby ModelMethods.GetCurrentlyConsumedQuantity(element) descending
                        select new { id = element.ID, name = element.GetFullName() };
        return Json(customers);
      }
      else
      {
        var customers = from element in Customers.GetAll().ToList()
                        where !element.Archived
                        orderby element.LastName
                        select new { id = element.ID, name = element.GetFullName() };
        return Json(customers);
      }
    }

    public JsonResult GetAllAccounts()
    {
      var accounts = from element in Accounts.GetAll().ToList()
                     orderby element.Name
                     select new { id = element.ID, name = element.Name };
      return Json(accounts);
    }

    public JsonResult GetProductCount(int customerId, int productId)
    {
      var customer = Customers.GetById(customerId);
      var product = Products.GetById(productId);

      var currentBill = customer.GetCurrentBill();
      int productCount = 0;
      foreach (var order in currentBill.Orders)
      {
        if (order.ProductName == product.Name)
        {
          productCount += order.Quantity;
        }
      }

      return Json(productCount);
    }

    public JsonResult OrderProduct(int customerId, int productId, int quantity)
    {
      Bill bill = Customers.GetById(customerId).GetCurrentBill();
      Product product = Products.GetById(productId);

      if (quantity > 0)
      {
        var order = new Order() { ProductName = product.Name, PurchaseDate = DateTime.Now, PricePerUnit = product.Price, Quantity = quantity, Bill = bill };
        Orders.Create(order);
      }

      return Json("OK");
    }

    public JsonResult OrderSpendedProduct(int spendingId, int quantity)
    {
      ProductSpending spending = ProductSpendings.GetById(spendingId);

      if (quantity > 0)
      {
        var order = new Order() { ProductName = spending.Product.Name, PurchaseDate = DateTime.Now, PricePerUnit = spending.Product.Price, Quantity = quantity, Bill = spending.Customer.GetCurrentBill() };
        Orders.Create(order);

        spending.Orders.Add(order);
        ProductSpendings.Update(spending);
      }

      return Json(new
      {
        id = spending.ID,
        productId = spending.Product.ID,
        customerName = spending.Customer.GetFullName(),
        customerId = spending.Customer.ID,
        quantity = spending.MaxQuantity,
        currentQuota = spending.GetAlreadyConsumed()
      });
    }

    // Undo the last order of given product
    public JsonResult UndoOrderProduct(int customerId, int productId)
    {
      Bill bill = Customers.GetById(customerId).GetCurrentBill();
      Product product = Products.GetById(productId);

      var orders = from element in bill.Orders.ToList()
                   where element.ProductName == product.Name
                   orderby element.PurchaseDate descending
                   select element;

      if (orders.Count() > 0)
      {
        Orders.Delete(orders.First());
      }

      return Json("OK");
    }

    public JsonResult UndoSpecificOrder(int orderId)
    {
      Orders.Delete(orderId);
      return Json("OK");
    }

    public JsonResult GetPastOrdersForProduct(int customerId, int productId)
    {
      Bill bill = Customers.GetById(customerId).GetCurrentBill();
      Product product = Products.GetById(productId);

      var orders = from element in bill.Orders.ToList()
                   where element.ProductName == product.Name
                   orderby element.PurchaseDate descending
                   select String.Format("{0} - {1}x {2}", element.PurchaseDate.ToString("dd.MM.yyyy HH:mm"), element.Quantity, element.ProductName);

      return Json(orders.Take(10));
    }

    public JsonResult AddAccount(string name)
    {
      var account = new Account() { Name = name };
      Accounts.Create(account);

      return Json(account.ID);
    }

    public JsonResult AddCustomer(string firstName, string lastName, int accountId)
    {
      if ((firstName + lastName).Trim().Length > 0)
      {
        var customer = new Customer() { FirstName = firstName, LastName = lastName };
        customer.Account = Accounts.GetById(accountId);
        customer.Bills.Add(new Bill() { Customer = customer, From = DateTime.Now });
        Customers.Create(customer);
      }

      return Json("OK");
    }

    public JsonResult AddProduct(string productName, decimal price)
    {
      var product = new Product() { Name = productName, Price = price };

      Products.Create(product);

      return Json("OK");
    }

    public JsonResult GetCurrentBills(int settlementId = -1)
    {
      var summary = new Dictionary<Account, Dictionary<Customer, List<OrderSummary>>>();
      if (settlementId == -1)
      {
        foreach (Account account in Accounts.GetAll())
        {
          var customerSummary = new Dictionary<Customer, List<OrderSummary>>();
          foreach (Customer customer in account.Customers)
          {
            List<OrderSummary> billSummary = ModelMethods.ComputeBillSummary(customer.GetCurrentBill());
            if (billSummary.Count > 0) customerSummary.Add(customer, billSummary);
          }
          summary.Add(account, customerSummary);
        }
      }
      else if (settlementId == -2)
      {
        foreach (Account account in Accounts.GetAll())
        {
          var customerSummary = new Dictionary<Customer, List<OrderSummary>>();
          foreach (Customer customer in account.Customers)
          {
            List<OrderSummary> billSummary = ModelMethods.ComputeBillsSummary(customer.Bills);
            if (billSummary.Count > 0) customerSummary.Add(customer, billSummary);
          }
          summary.Add(account, customerSummary);
        }
      }
      else
      {
        Settlement settlement = Settlements.GetById(settlementId);
        var accountGroups = from element in settlement.Bills
                            group element by element.Customer.Account into g
                            select g;
        foreach (Bill bill in settlement.Bills)
        {
          List<OrderSummary> billSummaries = ModelMethods.ComputeBillSummary(bill);
          if (billSummaries.Count > 0)
          {
            if (!summary.ContainsKey(bill.Customer.Account))
            {
              summary.Add(bill.Customer.Account, new Dictionary<Customer, List<OrderSummary>>());
            }
            summary[bill.Customer.Account].Add(bill.Customer, new List<OrderSummary>());
            foreach (var billSummary in billSummaries)
            {
              summary[bill.Customer.Account][bill.Customer].Add(billSummary);
            }
          }
        }
      }

      var jsonSummary = from element in summary
                        where summary[element.Key].Keys.Count > 0
                        orderby element.Key.Name
                        select new
                        {
                          accountName = element.Key.Name,
                          customerBills = from order in summary[element.Key]
                                          select new
                                          {
                                            customerName = order.Key.GetFullName(),
                                            billSummary = from orderSummary in summary[element.Key][order.Key]
                                                          select new
                                                          {
                                                            productName = orderSummary.ProductName,
                                                            quantity = orderSummary.Quantity,
                                                            totalPrice = orderSummary.TotalPrice
                                                          }
                                          }
                        };

      return Json(jsonSummary);
    }

    public ActionResult DownloadSettlement(int settlementId)
    {
      string file = CreateSettlementFile(Settlements.GetById(settlementId));

      return File(file, "application/vnd.ms-excel", string.Format("Abrechnung_am_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
    }

    public ActionResult BillAllAccounts()
    {
      var settlement = new Settlement();
      settlement.Created = DateTime.Now;
      foreach (Account account in Accounts.GetAll())
      {
        foreach (Customer customer in account.Customers)
        {
          Bill currentBill = customer.GetCurrentBill();
          currentBill.To = settlement.Created;
          customer.Bills.Add(new Bill() { From = settlement.Created, Customer = customer });

          settlement.Bills.Add(currentBill);
        }
      }

      Settlements.Create(settlement);

      string file = CreateSettlementFile(settlement);

      return File(file, "application/vnd.ms-excel", string.Format("Abrechnung_am_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
    }

    private string CreateSettlementFile(Settlement settlement)
    {
      string file = System.IO.Path.GetTempFileName();
      var workbook = new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("Abrechnung");
      worksheet.Cell("A1").Value = string.Format("Abrechnung am {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

      int currentRow = 3;
      var accountGroups = from element in settlement.Bills
                          where element.Orders.Count > 0
                          group element by element.Customer.Account into g
                          select g;
      foreach (var group in accountGroups)
      {
        decimal totalAccount = 0;
        foreach (Bill bill in group)
        {
          decimal totalCustomer = 0;
          List<OrderSummary> billSummaries = ModelMethods.ComputeBillSummary(bill);
          foreach (OrderSummary summary in billSummaries)
          {
            totalCustomer += summary.TotalPrice;
          }
          worksheet.Cell("A" + currentRow).Value = bill.Customer.GetFullName();
          worksheet.Cell("B" + currentRow).Style.NumberFormat.Format = "#.#0 €";
          worksheet.Cell("B" + currentRow).Value = totalCustomer;
          currentRow++;
          foreach (OrderSummary summary in billSummaries)
          {
            worksheet.Cell("A" + currentRow).Value = string.Format("{0}x {1}",
              summary.Quantity, summary.ProductName, summary.TotalPrice);

            worksheet.Cell("B" + currentRow).Style.NumberFormat.Format = "#.#0 €";
            worksheet.Cell("B" + currentRow).Value = summary.TotalPrice;

            currentRow++;
          }
          totalAccount += totalCustomer;
        }
        worksheet.Cell("A" + currentRow).Style.Font.SetBold();
        worksheet.Cell("A" + currentRow).Value = "Konto: " + group.First().Customer.Account.Name;
        worksheet.Cell("B" + currentRow).Style.Font.SetBold();
        worksheet.Cell("B" + currentRow).Style.NumberFormat.Format = "#.#0 €";
        worksheet.Cell("B" + currentRow).Value = totalAccount;
        currentRow += 2;
      }

      worksheet.Columns().AdjustToContents();
      workbook.SaveAs(file);

      return file;
    }

    public JsonResult SpendProduct(int customerId, int productId, int quantity)
    {
      var spending = new ProductSpending()
      {
        Customer = Customers.GetById(customerId),
        Product = Products.GetById(productId),
        MaxQuantity = quantity
      };
      ProductSpendings.Create(spending);

      return Json("OK");
    }

    public JsonResult SendSpendMail(int customerId, int productId, int quantity)
    {
      Customer customer = Customers.GetById(customerId);
      Product product = Products.GetById(productId);

      string from = ConfigurationManager.AppSettings["from"];
      string mailList = ConfigurationManager.AppSettings["mailList"];
      if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(mailList))
      {
        var message = new MailMessage();
        message.From = new MailAddress(from);
        message.To.Add(new MailAddress(mailList));
        string name = string.Format("{0} {1}", customer.FirstName, customer.LastName);
        message.Subject = string.Format("[BRMDrinks] {0} Klammer [{1}] {2} von {3}",
          DateTime.Now.ToString("dd.MM.yyyy HH:mm"), quantity, product.Name, name);

        var smtp = new SmtpClient();
        smtp.Send(message);
      }

      return Json("OK");
    }

    public JsonResult GetProductSpendings()
    {
      var spendings = from element in ProductSpendings.GetAll().ToList()
                      where element.GetAlreadyConsumed() < element.MaxQuantity
                      select new
                      {
                        id = element.ID,
                        productId = element.Product.ID,
                        customerName = element.Customer.GetFullName(),
                        customerId = element.Customer.ID,
                        quantity = element.MaxQuantity,
                        currentQuota = element.GetAlreadyConsumed()
                      };
      return Json(spendings);
    }

    public JsonResult GetSpendingCount(int spendingId)
    {
      var spending = ProductSpendings.GetById(spendingId);
      return Json(new { alreadyConsumed = spending.GetAlreadyConsumed(), maxQuantity = spending.MaxQuantity });
    }

    public JsonResult GetOrdersForSpending(int spendingId)
    {
      ProductSpending spending = ProductSpendings.GetById(spendingId);
      var orders = from element in spending.Orders
                   orderby element.PurchaseDate descending
                   select String.Format("{0} - {1}x {2}", element.PurchaseDate.ToString("dd.MM.yyyy HH:mm"), element.Quantity, element.ProductName);

      return Json(orders.Take(10));
    }

    public JsonResult GetOrdersForCustomer(int customerId)
    {
      var orders = from element in Customers.GetById(customerId).GetCurrentBill().Orders
                   orderby element.PurchaseDate descending
                   select new { id = element.ID, text = string.Format("{0} - {1}x {2}", element.PurchaseDate.ToString("dd.MM.yyyy HH:mm"), element.Quantity, element.ProductName) };
      return Json(orders);
    }

    public JsonResult GetAllSettlements()
    {
      var settlements = from element in Settlements.GetAll()
                        select new
                        {
                          id = element.ID,
                          created = element.Created.ToString("dd.MM.yyyy HH:mm"),
                        };

      return Json(settlements);
    }

    public JsonResult GetConsumption(string firstDay, string lastDay)
    {
      string[] formats = { "dd.MM.yyyy", "d.MM.yyyy", "dd.M.yyyy", "d.M.yyyy" };
      DateTime first;
      DateTime.TryParseExact(firstDay, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out first);
      DateTime last;
      DateTime.TryParseExact(lastDay, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out last);
      var orders = from element in Orders.GetAll()
                   where element.PurchaseDate >= first && element.PurchaseDate <= last
                   select element;
      var consumption = new Dictionary<DateTime, Dictionary<string, int>>();
      foreach (Order order in orders)
      {
        DateTime date = new DateTime(order.PurchaseDate.Year, order.PurchaseDate.Month, order.PurchaseDate.Day);
        if (!consumption.ContainsKey(date)) consumption.Add(date, new Dictionary<string, int>());
        if (!consumption[date].ContainsKey(order.ProductName)) consumption[date].Add(order.ProductName, order.Quantity);
        else consumption[date][order.ProductName] = consumption[date][order.ProductName] + order.Quantity;
      }

      var transportObject = from element in consumption
                            orderby element.Key
                            group element by element.Key into g
                            select new
                            {
                              date = g.Key.ToString("dd.MM.yyyy"),
                              products = from prod in consumption[g.Key]
                                         select new { name = prod.Key, value = prod.Value }
                            };

      return Json(transportObject);
    }

    public ActionResult DoBackup()
    {
      var settlement = new Settlement();
      settlement.Created = DateTime.Now;
      foreach (Account account in Accounts.GetAll())
      {
        foreach (Customer customer in account.Customers)
        {
          Bill currentBill = customer.GetCurrentBill();
          settlement.Bills.Add(currentBill);
        }
      }

      string file = CreateSettlementFile(settlement);
      System.IO.File.Copy(file, Path.Combine(ConfigurationManager.AppSettings["backupDir"], string.Format("brmdrinks_backup_{0}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm"))));
      return Content("OK");
    }

    public JsonResult RemoveProductSpending(int spendingId)
    {
      var spending = ProductSpendings.GetById(spendingId);
      foreach (var order in spending.Orders)
      {
        Orders.Create(new Order()
        {
          Bill = order.Bill,
          PricePerUnit = order.PricePerUnit,
          ProductName = order.ProductName,
          PurchaseDate = order.PurchaseDate,
          Quantity = order.Quantity
        });
      }
      ProductSpendings.Delete(spending);
      return Json("OK");
    }

    public JsonResult GetArchivedCustomers()
    {
      var customers = from element in Customers.GetAll().ToList()
                      where element.Archived
                      orderby element.LastName
                      select new
                      {
                        id = element.ID,
                        name = element.GetFullName(),
                      };
      return Json(customers);
    }
  }
}
