using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.VAS.Excel2Json.Model
{
  public class HItem
  {
    public string Name;
    public List<HItem> Children = new List<HItem>();
  }
}
