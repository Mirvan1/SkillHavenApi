using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Services
{
    public interface IUtilService
    {
         string SavePhoto(string photoBase64, string name);
        string GetPhotoAsBase64(string path);

        bool isPasswordEqual(string password, string confirmPassword);

        decimal RateCalculator(int userId);
    }
}
