using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared.Skill;

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
            var result = await _mediator.Send(query);
            return Ok(result);
        }



        [HttpGet("GetSupervisors")]
        public async Task<IActionResult> GetSupervisors([FromQuery]GetSupervisorsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("GetConsultants")]
        public async Task<IActionResult> GetConsultants([FromQuery] GetConsultantsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetSkiller/{UserId}")]
        public async Task<IActionResult> GetSkiller(int UserId)
        {
            var result = await _mediator.Send(new GetSkillerQuery() { UserId = UserId });
            return Ok(result);
        }

    }
}
