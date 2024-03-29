﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class DeleteBlogCommentCommand : IRequest<bool>
    {
        public int BlogCommentId { get; set; }
    }
}
