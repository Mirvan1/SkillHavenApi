using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.User
{
    public class RegisterUserCommand : IRequest<int>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; } = Role.User;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public ConsultantRegistrationInfo ConsultantInfo { get; set; } = null;
        public SupervisorRegistrationInfo SupervisorInfo { get; set; } = null;
    }

    public class ConsultantRegistrationInfo
    {
        public int Experience { get; set; }
        public string Description { get; set; }
    }

    public class SupervisorRegistrationInfo
    {
        public string Expertise { get; set; }
        public string Description { get; set; }

    }

    public enum Role
    {
        Admin = 1,
        User = 2,
        Consultant = 3,
        Supervisor = 4
    }
}
