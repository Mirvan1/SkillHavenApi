using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillHaven.Shared.User;

namespace SkillHaven.Shared.Skill
{
    public class SkillerDto
    {
        public Role? role { get; set; }
        public string FullName { get; set; }

        public string? SupervisorExpertise { get; set; }
        public string? SupervisorDescription { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
        public int? Experience { get; set; }
        public string? Description { get; set; }

        public decimal? Rating { get; set; }

        public int UserId { get; set; }
    }
}
