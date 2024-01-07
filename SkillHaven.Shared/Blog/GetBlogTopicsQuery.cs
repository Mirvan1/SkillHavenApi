﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class GetBlogTopicsQuery : PaginatedRequest, IRequest<PaginatedResult<GetBlogTopicsDto>>
    {
    }
}
