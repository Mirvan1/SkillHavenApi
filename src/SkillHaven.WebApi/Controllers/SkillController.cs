using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared.Skill;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SkillHaven.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillController : BaseController
    {
        public SkillController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("GetAllSkiller")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllSkillerQuery query)
        {
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }



        [HttpGet("GetSupervisors")]
        public async Task<IActionResult> GetSupervisors([FromQuery]GetSupervisorsQuery query)
        {
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }


        [HttpGet("GetConsultants")]
        public async Task<IActionResult> GetConsultants([FromQuery] GetConsultantsQuery query)
        {
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }

        [HttpGet("GetSkiller/{UserId}")]
        public async Task<IActionResult> GetSkiller(int UserId)
        {
            return Ok(await _mediator.Send(new GetSkillerQuery() { UserId=UserId }));
        }

    }
}
