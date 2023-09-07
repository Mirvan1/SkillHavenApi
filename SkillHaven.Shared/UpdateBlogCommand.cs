using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class UpdateBlogCommand:IRequest<bool>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool isPublished { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
