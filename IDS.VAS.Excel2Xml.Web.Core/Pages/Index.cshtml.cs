using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.CompilerServices;
using IDS.VAS.Excel2Xml.Automate;
using Ionic.Zip;

namespace IDS.VAS.Excel2Xml.Web.Core.Pages
{
  public class IndexModel : PageModel
  {
    private IWebHostEnvironment _hostingEnvironment;

    public IndexModel(IWebHostEnvironment hostingEnvironment)
    {
      _hostingEnvironment = hostingEnvironment;
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync(IEnumerable<IFormFile> files)
    {
      var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
      var dirInput = Path.Combine(_hostingEnvironment.WebRootPath, stamp);
      if (!Directory.Exists(dirInput))
        Directory.CreateDirectory(dirInput);

      var fileInput = Path.Combine(dirInput, "input.xlsx");

      using (var stream = new FileStream(fileInput, FileMode.Create, FileAccess.Write))
        await files.First().CopyToAsync(stream);

      var controller = new ConvertController();
      controller.Convert(fileInput, dirInput);

      var fileOutput = Path.Combine(_hostingEnvironment.WebRootPath, $"{stamp}.zip");

      using (var zipFile = new ZipFile())
      {
        zipFile.AddDirectory(dirInput);
        zipFile.Save(fileOutput);
      }

      return PhysicalFile(fileOutput, "application/zip", "download.zip");
    }
  }
}