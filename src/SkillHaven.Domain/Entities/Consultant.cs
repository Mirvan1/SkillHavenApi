using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class Consultant
    {
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public int Experience { get; set; }
        public string? Description { get; set; }
        public int? Rating { get; set; }
        public User User { get; set; }

    }

}
