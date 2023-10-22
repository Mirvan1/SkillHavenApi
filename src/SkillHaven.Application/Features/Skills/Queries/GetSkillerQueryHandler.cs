using AutoMapper;
using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using SkillHaven.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Skills.Queries
{
    public class GetSkillerQueryHandler : IRequestHandler<GetSkillerQuery, SkillerDto>
    {
        private readonly ISupervisorRepository _supervisorRepository;
        private readonly IConsultantRepository _consultantRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public GetSkillerQueryHandler(ISupervisorRepository supervisorRepository, IConsultantRepository consultantRepository, IMapper mapper, IUserService userService, IUserRepository userRepository)
        {
            _supervisorRepository=supervisorRepository;
            _consultantRepository=consultantRepository;
            _mapper=mapper;
            _userService=userService;
            _userRepository=userRepository;
        }

        public Task<SkillerDto> Handle(GetSkillerQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User do not authenticate");

            var getUser = _userRepository.GetById(request.UserId, x => x.Supervisor, y => y.Consultant);
            if (getUser is null) throw new ArgumentNullException("User cannot find");

            if (getUser.Role==Role.User.ToString() || getUser.Role==Role.Admin.ToString()) throw new UnauthorizedAccessException("The user don t have any skill");

            // var user = _mapper.Map<SkillerDto>(getUser);
            SkillerDto getSkiller = new()
            {
                role=(Role)Enum.Parse(typeof(Role), getUser.Role, true),
                Email=getUser.Email,
                FullName=getUser.FirstName+" "+getUser.LastName,
                Experience=getUser.Role==Role.Consultant.ToString() ? getUser.Consultant.Experience : 0,
                SupervisorDescription=getUser.Role==Role.Supervisor.ToString() ? getUser.Supervisor.Description : null,
                SupervisorExpertise=getUser.Role==Role.Supervisor.ToString() ? getUser.Supervisor.Description : null,

            };

            if (getUser.Role==Role.Supervisor.ToString())
            {
                getSkiller.Rating=getUser.Supervisor.Rating;
            }
            if (getUser.Role==Role.Consultant.ToString())
            {
                getSkiller.Rating=getUser.Consultant.Rating;
            }

            return Task.FromResult(getSkiller);
        }
    }
}
