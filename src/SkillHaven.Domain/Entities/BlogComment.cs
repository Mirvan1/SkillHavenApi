using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class BlogComments
    {
        public int BlogCommentsId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public string CommentTitle { get; set; }
        public string CommentContent { get; set; }
        public DateTime PublishDate { get; set; }
        public bool isPublished { get; set; }
        public virtual Blog Blog { get; set; }

        public virtual User User { get; set; }
    }
}
