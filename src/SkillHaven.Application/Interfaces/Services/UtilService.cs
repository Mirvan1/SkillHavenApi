using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Shared.User;
using SkillHaven.Shared.User.Mail;
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
        private readonly IUserRepository _userRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogVoteRepository _blogVoteRepository;
        private readonly SkillRater skillRater;
        //public UtilService(IConfiguration configuration)
        //{
        //    _configuration=configuration;
        // }

        public UtilService(IUserRepository userRepository, IBlogRepository blogRepository, IBlogVoteRepository blogVoteRepository, IOptions<SkillRater> skillRaterOption, IConfiguration configuration)
        {
            _userRepository=userRepository;
            _blogRepository=blogRepository;
            _blogVoteRepository=blogVoteRepository;
            skillRater= skillRaterOption.Value;
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
                if ( !string.IsNullOrEmpty(path) && path.Contains(PhotoTypes.UserPhoto.ToString()))
                    imageArray = System.IO.File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, $"{imagesFolderPath}/user.png"));
                else 
                    imageArray = System.IO.File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, $"{imagesFolderPath}/blog.png"));
            }
            string base64Image = Convert.ToBase64String(imageArray);
            string base64Format= "data:image/png;base64,";

            return base64Format+base64Image;
        }



      public async Task<decimal> RateCalculator(int userId,CancellationToken ct)
        {


            var getUser = _userRepository.GetById(userId);

            if (getUser is null) throw new ArgumentNullException("User cannot found");

            if(getUser.Role.Equals(Role.Consultant) || getUser.Role.Equals( Role.Supervisor))
                throw new ArgumentNullException("The user has no rater");

            var blogs = await _blogRepository.GetAllAsync(ct, x => x.UserId==getUser.UserId);

            if (blogs is null) throw new ArgumentNullException("Blog cannot found");

            decimal totalBlogCount = (decimal) Task.FromResult(blogs).Result.Count()/(decimal)skillRater.NormBlog;
            decimal? totalReaded = (decimal)blogs.Sum(x => x.NOfReading)/(decimal)skillRater.NormRead;
            decimal totalVotes = await getAllBlogVoteUser(blogs.Select(x => x.BlogId).ToList(),ct)/(decimal)skillRater.NormVote;

            decimal averageTotals =(decimal) (totalBlogCount+totalReaded+totalVotes)/3;
            return averageTotals*5;
        }


        public bool isPasswordEqual(string password, string confirmPassword)
        => password.Equals(confirmPassword);


        private async Task<decimal> getAllBlogVoteUser( List<int> totalBlogIds,CancellationToken ct)
        {
            decimal totalVote = 0;
            if(totalBlogIds!=null && totalBlogIds.Count()>0)
            {
                foreach( var blogId in totalBlogIds)
                {
                    totalVote+=await _blogVoteRepository.VotesByBlog(blogId,ct);
                }
            }
            return totalVote;
        }

    }
}
