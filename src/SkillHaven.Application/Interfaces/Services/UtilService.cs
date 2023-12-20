using Microsoft.Extensions.Configuration;
using SkillHaven.Shared.UtilDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SkillHaven.Application.Interfaces.Services
{
    public class UtilService : IUtilService
    {
        private readonly IConfiguration _configuration;

        public UtilService(IConfiguration configuration)
        {
            _configuration=configuration;
        }

        public string SavePhoto(string photoBase64, string name)
        {
            if (string.IsNullOrEmpty(photoBase64))
                return null;

            photoBase64=photoBase64.Replace("data:image/png;base64,", "");
            string imagesFolderPath = _configuration["ImagesFolderPath"];
            string extension = _configuration["ImageExtension"];
            string currentDirectory = Environment.CurrentDirectory;
            string targetDirectory = Path.Combine(Environment.CurrentDirectory, imagesFolderPath);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            byte[] imageBytes = Convert.FromBase64String(photoBase64);

            string fileName = name+"_"+DateTime.Now.ToFileTime();

            string filePath = Path.Combine(imagesFolderPath, $"{fileName}.{extension}");

            File.WriteAllBytes(filePath, imageBytes);

            return filePath;
        }

        public string GetPhotoAsBase64(string path)
        {
            string imagesFolderPath = _configuration["ImagesFolderPath"];
            string extension = _configuration["ImageExtension"];
            byte[] imageArray = null;
            try
            {
                  imageArray = System.IO.File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, $"{path}"));
            }
            catch (Exception e)
            {
                if (path.Contains(PhotoTypes.UserPhoto.ToString()))
                    imageArray = System.IO.File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, $"{imagesFolderPath}/user.png"));
                else 
                    imageArray = System.IO.File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, $"{imagesFolderPath}/blog.png"));
            }
            string base64Image = Convert.ToBase64String(imageArray);
            string base64Format= "data:image/png;base64,";

            return base64Format+base64Image;
        }
    }
}
