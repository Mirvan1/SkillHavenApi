using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class Supervisor
    {
        public int SupervisorId { get; set; }
        public int UserId { get; set; }
        public string Expertise { get; set; }
        public string? Description { get; set; }
        public decimal? Rating { get; set; }
        public virtual User User { get; set; }

    }
}
