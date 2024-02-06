using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class GetBlogTopicsDto
    {
        public string TopicName { get; set; }
        public bool IsActive { get; set; } = true;
        public int BlogTopicId { get; set; }
    }
}
