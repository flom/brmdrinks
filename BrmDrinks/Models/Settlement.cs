using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrmDrinks.Models
{
  public class Settlement : Base
  {
    public DateTime Created { get; set; }

    public virtual ICollection<Bill> Bills { get; set; }

    public Settlement()
    {
      Bills = new List<Bill>();
    }
  }
}