using AutoMapper;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.Chat;
using SkillHaven.Shared.Skill;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //CreateMap<GetUserQuery, UserDto>();
            //CreateMap<UserDto, GetUserQuery>();
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, SkillerDto>().ReverseMap();
            CreateMap<Supervisor, SkillerDto>().ReverseMap();
            CreateMap<Consultant, SkillerDto>().ReverseMap();

            CreateMap<Blog, GetBlogsDto>().ReverseMap();

            CreateMap<BlogComments, BlogCommentDto>().ReverseMap();
            CreateMap<ChatUser, GetOnlineUsersDto>().ReverseMap();
            CreateMap<ChatUser, GetChatUserDto>().ReverseMap();

        }
    }
}
