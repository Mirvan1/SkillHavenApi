﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class GetBlogQuery : IRequest<GetBlogsDto>
    {
        public int Id { get; set; }
    }
}
