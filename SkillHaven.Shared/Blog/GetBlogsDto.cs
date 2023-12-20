using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillHaven.Shared.User;

namespace SkillHaven.Shared.Blog
{
    public class GetBlogsDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; }
        public bool isPublished { get; set; }
        public string FullName { get; set; }
        public Role? Role { get; set; }
        public string Email { get; set; }
        public int? Vote { get; set; }
        public int? NOfReadings { get; set; }
        public int BlogComments { get; set; }
        public string? PhotoPath { get; set; }
    }
}
