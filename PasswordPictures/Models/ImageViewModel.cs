using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordPictures.Web.Models
{
    public class ImageViewModel
    {
        public string Password { get; set; }
        public bool CheckForPassword { get; set; }
        public string ImagePath { get; set; }
        public int ImageViewCount { get; set; }
        public int Id { get; set; }
        public string ErrorMessage { get; set; }
    }
}
