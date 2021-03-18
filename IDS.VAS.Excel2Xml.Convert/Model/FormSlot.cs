using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.VAS.Excel2Xml.Model
{
  /*
  <forms>
    <akt>
        <prototype>
            <pattern >
                <pitem slot="prd"     syn="V" sem="bewirkte Folge" />
                <pitem slot="figure"  syn="SUB" />
                <pitem slot="reason"  syn="VOR+DAT" />
            </pattern>
  */
  public class FormSlot
  {
    public FormSlot(ExcelColumnMapper map, DataRow row)
    {
      Type = row.ItemArray[map.Mapping["PRÄDIKAT(TYP)"]].ToString();
      switch (Type.ToLower())
      {
        case "a":
          Type = "akt";
          break;
        case "m":
          Type = "med";
          break;
        case "p":
          Type = "pass";
          break;
      }

      Kwics.Add(row.ItemArray[map.Mapping["#"]].ToString(), row.ItemArray[map.Mapping["BELEG"]].ToString());
      Prd = row.ItemArray[map.Mapping["MUSTERPRÄDIKAT"]].ToString();
      Figure = row.ItemArray[map.Mapping["FIGUR(SYN)"]].ToString();
      Ground = row.ItemArray[map.Mapping["GRUND(SYN)"]].ToString();
      Effector = row.ItemArray[map.Mapping["AUSLÖSER(SYN)"]].ToString();
    }

    public Dictionary<string, string> Kwics { get; set; } = new Dictionary<string, string>();
    public string Type { get; set; }
    public string Prd { get; set; }
    public string Figure { get; set; }
    public string Ground { get; set; }
    public string Effector { get; set; }

    public string GetXml(int addSamples)
    {
      var res = new StringBuilder();
      res.Append("<prototype>\r\n\t\t\t\t<pattern>\r\n");
      if (Test(Prd))
        res.Append($"\t\t\t\t\t<pitem slot=\"prd\" syn=\"{Prd}\" sem=\"TODO_PRD_SEM\"/>\r\n");
      if (Test(Effector))
        res.Append($"\t\t\t\t\t<pitem slot=\"effector\" syn=\"{Effector}\"/>\r\n");
      if (Test(Figure))
        res.Append($"\t\t\t\t\t<pitem slot=\"figure\" syn=\"{Figure}\"/>\r\n");
      if (Test(Ground))
        res.Append($"\t\t\t\t\t<pitem slot=\"ground\" syn=\"{Ground}\"/>\r\n");
      res.Append("\t\t\t\t</pattern>\r\n");

      if (addSamples > 0) // wenn addSamples == 0 dann gar keine Belege (wichtig für beleglosen Vergleich)
      {
        res.Append("\t\t\t<!-- TODO: Gewünschte Belege auskommentieren. Nicht benötigte Belege einkommentieren oder ggf. löschen -->\r\n");
        res.Append("\t\t\t<examples>\r\n");
        foreach (var id in Kwics)
        {
          if (addSamples > 0) // zähle addSamples runter (wichtig für overview = 3 und predicate = 1)
          {
            res.Append($"\t\t\t\t<xref href=\"s_{id.Key}\"/> <!-- {id.Value} -->\r\n");
            addSamples--;
          }
          else
            res.Append($"\t\t\t\t<!-- <xref href=\"s_{id.Key}\"/> --> <!-- {id.Value} -->\r\n");
        }
        res.Append("\t\t\t</examples>\r\n");
      }
      res.Append("\t\t\t</prototype>\r\n");
      return res.ToString();
    }
    
    private bool Test(string property)
    {
      if (string.IsNullOrEmpty(property))
        return false;
      if (property == "-" || property == "–")
        return false;
      return true;
    }
  }
}
