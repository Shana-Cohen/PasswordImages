using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordPictures.Data;
using PasswordPictures.Web.Models;
//using PasswordPictures.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordPictures.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImageDB;Integrated Security=true;Encrypt=False;";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", fileName);
            using var fs = new FileStream(path, FileMode.CreateNew);
            image.CopyTo(fs);
            var repo = new ImageRepo(_connectionString);
            var newImage = new Image
            {
                FileName = fileName,
                Password = password,
                Views = 1
            };
            newImage.Id = repo.AddImage(newImage);
            return View(newImage);
;
        }

        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepo(_connectionString);
            var image = repo.GetImage(id);
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (ids.Contains(image.Id))
            {
                repo.IncrementViews(id);
            }
            var vm = new ImageViewModel
            {
                Id = id,
                CheckForPassword = !ids.Contains(image.Id),
                ImagePath = image.FileName,
                ImageViewCount = image.Views,
                ErrorMessage = (string)TempData["message"]
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var repo = new ImageRepo(_connectionString);
            var idList = HttpContext.Session.Get<List<int>>("ids");
            if (!String.IsNullOrEmpty(password) && password == repo.GetPassword(id))
            {
                if (idList == null)
                {
                    idList = new List<int>();
                }
                idList.Add(id);
                HttpContext.Session.Set("ids", idList);
            }
            else
            {
                TempData["message"] = "Invalid Password";
            }
            return Redirect($"/home/viewimage?id={id}");
        }

    }
    public static class SessionExtensionMethods
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
