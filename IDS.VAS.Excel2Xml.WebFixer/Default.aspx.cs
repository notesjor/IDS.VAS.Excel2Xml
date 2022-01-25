using System;
using System.IO;
using IDS.VAS.Excel2Xml.Automate;
using IDS.VAS.Excel2Xml.WebFixer;
using Ionic.Zip;
using Telerik.Web.UI;

public partial class Default : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
  }

  protected void btn_execute_Click(object sender, EventArgs e)
  {
    try
    {
      var dirBase = Server.MapPath("~/temp");
      var datBase = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

      var dirInput = Path.Combine(dirBase, datBase, "input");
      Directory.CreateDirectory(dirInput);
      var excelInput = Path.Combine(dirInput, "input.xlsx");

      var dirOutput = Path.Combine(dirBase, datBase, "output");
      Directory.CreateDirectory(dirOutput);

      foreach (UploadedFile file in upload_excel.UploadedFiles)
      {
        file.SaveAs(excelInput);
        break;
      }

      var controller = new ConvertController();
      controller.Convert(excelInput, dirOutput);

      foreach (UploadedFile file in upload_xml.UploadedFiles)
      {
        file.SaveAs(Path.Combine(dirInput, Path.GetFileName(file.FileName)));
        break;
      }

      var cherryPick = new CherryPickController();
      cherryPick.Process(dirInput, dirOutput);

      Response.Clear();
      Response.BufferOutput = false;

      Response.ContentType = "application/zip";
      Response.AddHeader("content-disposition",
                         $"attachment; filename=CorpusExplorerWebConvert-{DateTime.Now:yyyy-MM-dd-HHmmss}.zip");
      using (var zipFile = new ZipFile())
      {
        zipFile.AddDirectory(dirOutput);
        zipFile.Save(Response.OutputStream);
      }

      Response.Flush();
    }
    catch (Exception ex)
    {
      calc_error.Text = ex.Message;
    }
  }
}
