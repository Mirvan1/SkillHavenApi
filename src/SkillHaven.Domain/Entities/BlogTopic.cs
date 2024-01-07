using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class BlogTopic
    {
        public int BlogTopicId { get; set; }
        public string TopicName { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }

    }
}
