﻿using AseShop.Common.Infrastructure.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Skills.Queries
{
    public class GetSupervisorsQueryHandler : IRequestHandler<GetSupervisorsQuery, PaginatedResult<SkillerDto>>
    {
        private readonly ISupervisorRepository _supervisorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetSupervisorsQueryHandler(ISupervisorRepository supervisorRepository, IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper)
        {
            _supervisorRepository=supervisorRepository;
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
        }



        public Task<PaginatedResult<SkillerDto>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
        {

            Expression<Func<Supervisor, bool>> filterExpression = null;
            Func<IQueryable<Supervisor>, IOrderedQueryable<Supervisor>> orderByExpression = null;

            if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");


            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.User.FirstName.Contains(request.Filter);
            }

            var includeProperties = new Expression<Func<Supervisor, object>>[]
            {
                e => e.User,
            };

            var dbResult = _supervisorRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, includeProperties);

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
                        SupervisorDescription=data.Description,
                        SupervisorExpertise=data.Expertise,
                        ProfilePicture=data.User?.ProfilePicture,
                        role=Enum.TryParse(data.User?.Role, out Role r) ? r : null,
                        Email=data?.User?.Email
                    };
                    result.Data.Add(skillerDto);
                }
            }
            return Task.FromResult(result);
        }
    }
}