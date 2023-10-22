using AutoMapper;
using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Skills.Queries
{
    public class GetAllSkillerQueryHandler : IRequestHandler<GetAllSkillerQuery, PaginatedResult<SkillerDto>>
    {
        private readonly ISupervisorRepository _supervisorRepository;
        private readonly IConsultantRepository _consultantRepository;
        private readonly IMapper _mapper;


        public GetAllSkillerQueryHandler(ISupervisorRepository supervisorRepository, IConsultantRepository consultantRepository, IMapper mapper)
        {
            _supervisorRepository=supervisorRepository;
            _consultantRepository=consultantRepository;
            _mapper=mapper;
        }

        public Task<PaginatedResult<SkillerDto>> Handle(GetAllSkillerQuery query, CancellationToken cancellationToken)
        {
            PaginatedResult<SkillerDto> skillers = new();


            Expression<Func<Supervisor, bool>> filterExpressionSupervisor = null;
            Func<IQueryable<Supervisor>, IOrderedQueryable<Supervisor>> orderByExpressionSupervisor = null;

            if (!string.IsNullOrWhiteSpace(query.SearchByName))
            {
                filterExpressionSupervisor = entity => entity.User.FirstName.Contains(query.SearchByName);
            }

            var includePropertiesSupervisor = new Expression<Func<Supervisor, object>>[]
            {
                e => e.User,
            };
            var dbResultSupervisor = _supervisorRepository.GetPaged(query.Page, query.PageSize/2, query.OrderByPropertname, query.OrderBy, filterExpressionSupervisor, includePropertiesSupervisor);


            if (dbResultSupervisor is not null)
            {
                skillers.Data.AddRange(_mapper.Map<List<SkillerDto>>(dbResultSupervisor));
                skillers.TotalCount=dbResultSupervisor.TotalCount;
                skillers.TotalPages=dbResultSupervisor.TotalPages;
            }

            //var getAllSupervisors = _supervisorRepository.GetAll();//add pagination
            //if (getAllSupervisors is not null)
            //{
            //    var getSkillers = _mapper.Map<List<SkillerDto>>(getAllSupervisors);
            //    skillers.AddRange(getSkillers);
            //}



            Expression<Func<Consultant, bool>> filterExpressionConsultant = null;
            Func<IQueryable<Consultant>, IOrderedQueryable<Consultant>> orderByExpressionConsultant = null;

            if (!string.IsNullOrWhiteSpace(query.SearchByName))
            {
                filterExpressionConsultant = entity => entity.User.FirstName.Contains(query.SearchByName);
            }

            var includePropertiesConsultant = new Expression<Func<Consultant, object>>[]
            {
                e => e.User,
            };

            var dbResultConsultancy = _consultantRepository.GetPaged(query.Page, query.PageSize / 2, query.OrderByPropertname, query.OrderBy, filterExpressionConsultant, includePropertiesConsultant);
            if (dbResultConsultancy is not null)
            {
                skillers.Data.AddRange(_mapper.Map<List<SkillerDto>>(dbResultConsultancy));
                skillers.TotalCount+=dbResultConsultancy.TotalCount;
                skillers.TotalPages+=dbResultConsultancy.TotalPages;
            }


            //    var getAllConsultancy = _consultantRepository.GetAll();
            //if (getAllConsultancy is not null)
            //{
            //    var getSkillers = _mapper.Map<List<SkillerDto>>(getAllConsultancy);
            //    skillers.AddRange(getSkillers);
            //}

            return Task.FromResult(skillers);
        }
    }
}
