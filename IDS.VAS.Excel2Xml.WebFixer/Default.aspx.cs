using System;
using System.IO;
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
      var filInput = Path.Combine(dirInput, "input.xlsx");

      var dirOutput = Path.Combine(dirBase, datBase, "output");
      Directory.CreateDirectory(dirOutput);

      progress_convert.Value = 25;
      progress_convert.Label = "Pre-Processing";
      progress_convert.Visible = true;

      foreach (UploadedFile file in upload_files.UploadedFiles)
      {
        file.SaveAs(filInput);
        break;
      }

      progress_convert.Value = 50;
      progress_convert.Label = "Convert";

    //  var controller = new ConvertController();
    //  controller.Convert(filInput, dirOutput);

      progress_convert.Value = 75;
      progress_convert.Label = "Download";

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
      progress_convert.Value = 100;
      progress_convert.Label = "Done!";
    }
    catch
    {
      progress_convert.Value = 100;
      progress_convert.Label = "Error!";
    }
  }
}
