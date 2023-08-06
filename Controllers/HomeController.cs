using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Async_File_Upload.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public HomeController(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        //For the method to work, add (Name = "file") to the From form, since the client markup specifies fdata.append ("file", file);
        [HttpPost]
        [RequestSizeLimit(1048576)]
        public IActionResult Upload([FromForm(Name = "file")] IFormFile uploadfile)
        {
            try
            {
                if (uploadfile == null) throw new Exception("Вам нужно выбрать файл\r\n");
                var uniqueFileName = GetUniqueFileName(uploadfile.FileName);
                var uploads = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    uploadfile.CopyTo(fileStream);
                }
                var url = Url.Content("~/uploads/" + uniqueFileName);
                return Json(new { status = "success", url = url });
            }
            catch (Exception ex)
            {
                // to do : log error
                return Json(new { status = "error", message = ex.Message });
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }
}
