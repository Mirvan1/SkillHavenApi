using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SkillHaven.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SkillController : BaseController
    {
        public SkillController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllSkillerQuery();
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }



        [HttpGet("GetSupervisor")]
        public async Task<IActionResult> GetSupervisor([FromQuery]GetSupervisorsQuery query)
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



    }
}
