using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class BlogCommentDto
    {
        public int BlogCommentsId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public string CommentTitle { get; set; }
        public string CommentContent { get; set; }
        public DateTime PublishDate { get; set; }
        public bool isPublished { get; set; }
        public string FullName { get; set; }
        public string BlogName { get; set; }

        public string UserPhoto { get; set; }

    }
}
