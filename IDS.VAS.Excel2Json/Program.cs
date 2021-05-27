using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using ExcelDataReader;
using IDS.VAS.Excel2Json.Model;
using IDS.VAS.Excel2Xml.Model;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2Json
{
  class Program
  {
    private static DataSet _workbook;
    private static string _mainPattern;
    private static string _baseDir;

    static void Main(string[] args)
    {
      if (args.Length == 0)
        return;
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

      _baseDir = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]));

      ReadExcel(args[0]);
      ReadWorkbook();
    }

    private static void ReadExcel(string path)
    {
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        var reader = ExcelReaderFactory.CreateReader(fs, new ExcelReaderConfiguration()
        {
          // Gets or sets the encoding to use when the input XLS lacks a CodePage
          // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
          // Default: cp1252 (XLS BIFF2-5 and CSV only)
          FallbackEncoding = Encoding.GetEncoding(1252),

          // Gets or sets a value indicating whether to leave the stream open after
          // the IExcelDataReader object is disposed. Default: false
          LeaveOpen = false
        });

        _workbook = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
          // Gets or sets a value indicating whether to set the DataColumn.DataType 
          // property in a second pass.
          UseColumnDataType = true,

          // Gets or sets a callback to determine whether to include the current sheet
          // in the DataSet. Called once per sheet before ConfigureDataTable.
          FilterSheet = (tableReader, sheetIndex) => sheetIndex == 0,

          // Gets or sets a callback to obtain configuration options for a DataTable. 
          ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
          {
            // Gets or sets a value indicating the prefix of generated column names.
            EmptyColumnNamePrefix = "Column",

            // Gets or sets a value indicating whether to use a row from the 
            // data as column names.
            UseHeaderRow = true,

            // Gets or sets a callback to determine whether to include the 
            // current row in the DataTable.
            FilterRow = (rowReader) => { return true; },

            // Gets or sets a callback to determine whether to include the specific
            // column in the DataTable. Called once per column after reading the 
            // headers.
            FilterColumn = (rowReader, columnIndex) => { return true; }
          }
        });
      }
    }

    private static void ReadWorkbook()
    {
      foreach (DataTable sheet in _workbook.Tables)
      {
        var mapper = new ExcelColumnMapper();
        if (mapper.Map(sheet))
          continue;

        var outputDir = Path.Combine(_baseDir, sheet.TableName);
        if (!Directory.Exists(outputDir))
          Directory.CreateDirectory(outputDir);

        var idx = mapper.Mapping["MUSTER"];
        var patterns = new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[idx].ToString());
        var results = new List<FacetedSearchItem>();

        foreach (var pattern in patterns)
        {
          if (string.IsNullOrWhiteSpace(pattern))
            continue;

          var rows = sheet.Rows.Cast<DataRow>()
                           .Where(row => row.ItemArray[mapper.Mapping["MUSTER"]].ToString() == pattern)
                           .Where(row => row.ItemArray[mapper.Mapping["EINGANG"]].ToString().Trim() == "1")
                           .ToArray();

          var fsi = new FacetedSearchItem { Name = pattern, Mustertyp = rows.First().ItemArray[mapper.Mapping["MUSTERTYP"]]?.ToString() };
          foreach (var row in rows)
          {
            NewMethod(row, mapper, fsi.Quellen, "QUELLE");
            NewMethod(row, mapper, fsi.Jahre, "JAHR");
            //NewMethod(row, mapper, fsi.Belege, "BELEG");
            NewMethod(row, mapper, fsi.Prädikatskerne, "PRÄDIKATSKERN(LEX)");
            NewMethod(row, mapper, fsi.Prädikate, "PRÄDIKAT(LEX)", x => x.Replace("_", " "));
            NewMethod(row, mapper, fsi.Diathesen, "DIATHESE");
            NewMethod(row, mapper, fsi.KTypen, "KTYP");
            NewMethod(row, mapper, fsi.PrdMusterslot, "PRD(MUSTERSLOT)");
            NewMethod(row, mapper, fsi.Auslöser, "AUSLÖSER(SYN)");
            NewMethod(row, mapper, fsi.Figuren, "FIGUR(SYN)");
            NewMethod(row, mapper, fsi.GrundLex, "GRUND(LEX)");
            NewMethod(row, mapper, fsi.GrundSyn, "GRUND(SYN)");
          }
          results.Add(fsi);
        }

        // Speichern
        if (!Directory.Exists("output"))
          Directory.CreateDirectory("output");

        File.WriteAllText("output/all.json", JsonConvert.SerializeObject(results), Encoding.UTF8); // ALLE
        foreach (var item in results)
          File.WriteAllText($"output/{item.Name}.json", JsonConvert.SerializeObject(item), Encoding.UTF8); // EINZELN
        File.WriteAllText("output/meta_sources.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Quellen))), Encoding.UTF8);
        File.WriteAllText("output/meta_years.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Jahre))), Encoding.UTF8);
        //File.WriteAllText("output/meta_kwics.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Belege))), Encoding.UTF8);
        File.WriteAllText("output/meta_prd_core.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Prädikatskerne))), Encoding.UTF8);
        File.WriteAllText("output/meta_prd.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Prädikate))), Encoding.UTF8);
        File.WriteAllText("output/meta_diathesis.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Diathesen))), Encoding.UTF8);
        File.WriteAllText("output/meta_ktypes.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.KTypen))), Encoding.UTF8);
        File.WriteAllText("output/meta_prd_slot.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.PrdMusterslot))), Encoding.UTF8);
        File.WriteAllText("output/meta_trigger.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Auslöser))), Encoding.UTF8);
        File.WriteAllText("output/meta_figure.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.Figuren))), Encoding.UTF8);
        File.WriteAllText("output/meta_ground_lex.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.GrundLex))), Encoding.UTF8);
        File.WriteAllText("output/meta_grund_syn.json", JsonConvert.SerializeObject(new HashSet<string>(results.SelectMany(x => x.GrundSyn))), Encoding.UTF8);
      }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void NewMethod(DataRow row, ExcelColumnMapper mapper, HashSet<string> fsi, string name, Func<string, string> mod = null)
    {
      var val = row.ItemArray[mapper.Mapping[name]]?.ToString();
      if (!string.IsNullOrWhiteSpace(val))
        fsi.Add(mod == null ? val : mod(val));
    }


    private static string PatternNameFix(string pattern)
    {
      return pattern.Substring(0, 1).ToUpper() + pattern.Substring(1).ToLower();
    }
  }
}
