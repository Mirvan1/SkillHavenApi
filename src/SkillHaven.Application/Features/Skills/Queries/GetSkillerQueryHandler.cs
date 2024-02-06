using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Skill;
using SkillHaven.Shared.User;
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
        public readonly IStringLocalizer _localizer;
        private readonly IUtilService _utilService;

        public GetSkillerQueryHandler(ISupervisorRepository supervisorRepository, IConsultantRepository consultantRepository, IMapper mapper, IUserService userService, IUserRepository userRepository, IUtilService utilService)
        {
            _supervisorRepository=supervisorRepository;
            _consultantRepository=consultantRepository;
            _mapper=mapper;
            _userService=userService;
            _userRepository=userRepository;
            _localizer=new Localizer();
            _utilService=utilService;
        }

        public async Task<SkillerDto> Handle(GetSkillerQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var getUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken,x => x.Supervisor, y => y.Consultant);
            if (getUser is null) throw new ArgumentNullException("User cannot find");

            if (getUser.Role==Role.User.ToString() || getUser.Role==Role.Admin.ToString()) throw new UnauthorizedAccessException("The user don t have any skill");

            // var user = _mapper.Map<SkillerDto>(getUser);
            SkillerDto getSkiller = new()
            {
                role=(Role)Enum.Parse(typeof(Role), getUser.Role, true),
                Email=getUser.Email,
                FullName=getUser.FirstName+" "+getUser.LastName,
                Experience=getUser.Role==Role.Consultant.ToString() ? getUser.Consultant.Experience : 0,
                Description=getUser.Role == Role.Consultant.ToString()?getUser.Consultant.Description:null,
                SupervisorDescription=getUser.Role==Role.Supervisor.ToString() ? getUser.Supervisor.Description : null,
                SupervisorExpertise=getUser.Role==Role.Supervisor.ToString() ? getUser.Supervisor.Description : null,
                ProfilePicture=_utilService.GetPhotoAsBase64(getUser.ProfilePicture)
            };

            if (getUser.Role==Role.Supervisor.ToString())
            {
                getSkiller.Rating=await _utilService.RateCalculator(getUser.UserId,cancellationToken);//getUser.Supervisor.Rating;
            }
            if (getUser.Role==Role.Consultant.ToString())
            {
                getSkiller.Rating=await _utilService.RateCalculator(getUser.UserId,cancellationToken); // getUser.Consultant.Rating;
            }

            return getSkiller;
        }
    }
}
