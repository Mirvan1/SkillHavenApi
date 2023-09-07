using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class SkillerDto
    {
        public Role? role { get; set; }
        public string FullName { get; set; }

        public string SupervisorExpertise { get; set; }
        public string SupervisorDescription { get; set; }
        public string? Email { get; set; }
        public string ProfilePicture { get; set; }
        public int Experience { get; set; }
        public string Description { get; set; }
    }
}
