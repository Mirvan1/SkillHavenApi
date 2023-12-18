using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }

        public bool IsDeleted { get; set; }
        public string? MailConfirmationCode { get; set; }
        public bool? HasMailConfirm { get; set; }

        // Navigation properties for relationships
        public Supervisor? Supervisor { get; set; }
        public Consultant? Consultant { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<BlogComments> BlogComments { get; set; }

    }
}
