using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class CreateBlogCommentCommand : IRequest<bool>
    {      
        public int BlogId { get; set; }
        public string CommentTitle { get; set; }
        public string CommentContent { get; set; }
        public DateTime PublishDate { get; set; }
        public bool isPublished { get; set; }
    }
}
