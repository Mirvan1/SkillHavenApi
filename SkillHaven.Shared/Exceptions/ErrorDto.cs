using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Exceptions;

public class ErrorDto
{
    public int StatusCode { get; set; } = 500;
    public string Message { get; set; } = "Internal Server Error";

    public string DetailMessage { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
