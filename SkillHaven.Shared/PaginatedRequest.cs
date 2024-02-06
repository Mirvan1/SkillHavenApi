using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class PaginatedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool OrderBy { get; set; } = false;
        public string OrderByPropertname { get; set; } = string.Empty;

        public string? Filter { get; set; } = string.Empty;
    }
}
