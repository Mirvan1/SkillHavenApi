using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class Blog
    {
        public int BlogId { get; set; } 
        public string Title { get; set; } 
        public string Content { get; set; }   
        public int UserId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
        public bool IsPublished { get; set; }
        public int? Vote { get; set; }
        public int? BlogTopicId { get; set; }

        public string? PhotoPath { get; set; }

        public long? NOfReading { get; set; }
        public virtual User User { get; set; }
        public virtual BlogTopic BlogTopics { get; set; }

        public virtual ICollection<BlogComments> BlogComments{ get; set;}
    }
}
