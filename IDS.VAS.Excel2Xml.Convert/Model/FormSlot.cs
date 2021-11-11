using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.VAS.Excel2Xml.Automate.Model;
using IDS.VAS.Excel2Xml.Convert.Model;

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
    public FormSlot(ExcelColumnMapper mapper, DataRow row)
    {
      Type = row.ItemArray[mapper.Mapping["DIATHESE"]].ToString();
      switch (Type.ToLower())
      {
        case "a":
          Type = "akt";
          break;
        case "k":
          Type = "kon";
          break;
        case "p":
          Type = "pass";
          break;
        case "ambig":
          Type = "ambig";
          break;
      }

      Kwics.Add(row.ItemArray[mapper.Mapping["#"]].ToString(), new Kwic
      {
        Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
        Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
        Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
        Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString(),
        MType = row.ItemArray[mapper.Mapping["MUSTERTYP"]].ToString(),
        KType = row.ItemArray[mapper.Mapping["KONSTRUKTIONSTYP"]].ToString(),
      });
      Prd = row.ItemArray[mapper.Mapping["PRD(SYN)"]].ToString();
      Figure = row.ItemArray[mapper.Mapping["FIGUR(SYN)"]].ToString();
      Ground = row.ItemArray[mapper.Mapping["GRUND(KOPF)"]] + "+" +  row.ItemArray[mapper.Mapping["GRUND(KASUS)"]];
      Effector = row.ItemArray[mapper.Mapping["AUSLÖSER(SYN)"]].ToString();
    }

    public Dictionary<string, Kwic> Kwics { get; set; } = new Dictionary<string, Kwic>();
    public string Type { get; set; }
    public string Prd { get; set; }
    public string Figure { get; set; }
    public string Ground { get; set; }
    public string Effector { get; set; }

    public string GetXml(int addSamples, bool useGenerator)
    {
      var res = new StringBuilder();
      res.Append($"<prototype>\r\n\t\t\t\t<pattern id=\"{(useGenerator ? IdGenerator.GetId("p") : IdGenerator.GetNullId("p"))}\">\r\n");
      if (!useGenerator)
      {
        res.Append($"\t\t\t\t\t<pitem slot=\"diathese\" syn=\"{Type}\"/>\r\n");
      }
      if (Test(Prd))
      {
        res.Append($"\t\t\t\t\t<pitem slot=\"rel\" syn=\"{Prd}\" sem=\"TODO_PRD_SEM\"/>\r\n");
        //res.Append($"\t\t\t\t\t<pitem slot=\"ktype\" syn=\"{Kwics.First().Value.KType}\"/>\r\n");
      }
      if (Test(Effector))
        res.Append($"\t\t\t\t\t<pitem slot=\"effector\" syn=\"{Effector}\"/>\r\n");
      if (Test(Figure))
        res.Append($"\t\t\t\t\t<pitem slot=\"figure\" syn=\"{Figure}\"/>\r\n");

      if (Test(Ground))
        res.Append($"\t\t\t\t\t<pitem slot=\"ground\" syn=\"{Ground}\"/>\r\n");
      res.Append("\t\t\t\t</pattern>\r\n");

      if (addSamples > 0) // wenn addSamples == 0 dann gar keine Belege (wichtig für beleglosen Vergleich)
      {
        res.Append("\t\t\t\t<!-- TODO: Gewünschte Belege auskommentieren. Nicht benötigte Belege einkommentieren oder ggf. löschen -->\r\n");
        res.Append("\t\t\t\t<examples>\r\n");
        foreach (var id in Kwics.OrderByDescending(x => x.Value.PriorityIndex))
        {
          if (addSamples > 0) // zähle addSamples runter (wichtig für overview = 3 und predicate = 1)
          {
            res.Append($"\t\t\t\t\t<xref href=\"s_{id.Key}\"/> <!-- {id.Value} -->\r\n");
            addSamples--;
          }
          else
            res.Append($"\t\t\t\t\t<!-- <xref href=\"s_{id.Key}\"/> --> <!-- {id.Value} -->\r\n");
        }
        res.Append("\t\t\t\t</examples>\r\n");
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
