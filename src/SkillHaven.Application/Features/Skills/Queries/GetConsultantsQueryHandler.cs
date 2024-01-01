using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Skill;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Skills.Queries
{
    public class GetConsultantsQueryHandler : IRequestHandler<GetConsultantsQuery, PaginatedResult<SkillerDto>>
    {
        private readonly IConsultantRepository _consultantRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;
        public readonly IUtilService _utilService;

        public GetConsultantsQueryHandler(IConsultantRepository consultantRepository, IUserService userService, IMapper mapper, IUtilService utilService)
        {
            _consultantRepository=consultantRepository;
            _userService=userService;
            _mapper=mapper;
            _localizer=new Localizer();
            _utilService=utilService;
        }
        public Task<PaginatedResult<SkillerDto>> Handle(GetConsultantsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Consultant, bool>> filterExpression = null;
            Func<IQueryable<Consultant>, IOrderedQueryable<Consultant>> orderByExpression = null;

            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);


            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.User.FirstName.Contains(request.Filter);
            }

            var includeProperties = new Expression<Func<Consultant, object>>[]
            {
                e => e.User,
            };

            var dbResult = _consultantRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, includeProperties);

            PaginatedResult<SkillerDto> result = new()
            {
                TotalCount=dbResult.TotalCount,
                TotalPages=dbResult.TotalPages,
            };

            if (dbResult.Data is not null)
            {
                foreach (var data in dbResult.Data)
                {
                    SkillerDto skillerDto = new()
                    {
                        FullName=data.User?.FirstName+" "+data.User?.LastName,
                        Description=data.Description,
                        Experience=data.Experience,
                        ProfilePicture=_utilService.GetPhotoAsBase64(data?.User?.ProfilePicture),
                        role=Enum.TryParse(data.User?.Role, out Role r) ? r : null,
                        Email=data?.User?.Email,
                        UserId=data.UserId,
                        Rating=_utilService.RateCalculator(data.UserId)
                    };
                    result.Data.Add(skillerDto);
                }
            }
            return Task.FromResult(result);
        }

    }

 
    
}
