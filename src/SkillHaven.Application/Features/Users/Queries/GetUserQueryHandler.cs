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

namespace SkillHaven.Application.Features.Users.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository  userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user =  _userRepository.GetById(request.UserId);

            if (user == null)
                throw new DatabaseValidationException("User cannot found");

            var userDto = _mapper.Map< UserDto>(user);

            return userDto;
        }

    
        //public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        //{
        //    var user =  _userRepository.GetById(request.UserId);

        //    if (user == null)
        //    {
        //        return null; // Veya uygun bir hata dönüşü yapabilirsiniz.
        //    }

        //    return new UserDto
        //    {
        //        UserId = user.UserId,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        Role = user.Role,
        //        ProfilePicture = user.ProfilePicture
        //    };
        //}
    }
    }
