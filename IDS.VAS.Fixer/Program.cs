using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.VAS.Excel2Xml.Automate;
using IDS.VAS.Excel2Xml.WebFixer;

namespace IDS.VAS.Fixer
{
  class Program
  {
    static void Main(string[] args)
    {
      var controller = new ConvertController();
      controller.Convert(args[0], args[2]);
      
      var cherryPick = new CherryPickController();
      cherryPick.Process(args[1], args[2]);
    }
  }
}
