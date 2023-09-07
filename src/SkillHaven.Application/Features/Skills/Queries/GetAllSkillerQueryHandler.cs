using AutoMapper;
using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Skills.Queries
{
    public class GetAllSkillerQueryHandler : IRequestHandler<GetAllSkillerQuery, List<SkillerDto>>
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

        public Task<List<SkillerDto>> Handle(GetAllSkillerQuery query, CancellationToken cancellationToken)
        {
            List<SkillerDto> skillers = new();
            var getAllSupervisors = _supervisorRepository.GetAll();//add pagination
            if (getAllSupervisors is not null)
            {
                var getSkillers = _mapper.Map<List<SkillerDto>>(getAllSupervisors);
                skillers.AddRange(getSkillers);
            }

            var getAllConsultancy = _consultantRepository.GetAll();
            if (getAllConsultancy is not null)
            {
                var getSkillers = _mapper.Map<List<SkillerDto>>(getAllConsultancy);
                skillers.AddRange(getSkillers);
            }

            return Task.FromResult(skillers);
                }
    }
}
