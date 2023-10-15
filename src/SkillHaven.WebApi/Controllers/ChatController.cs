using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared;

namespace SkillHaven.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ChatController : BaseController
    {
        public ChatController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("GetMessageByUser")]
        public async Task<IActionResult> GetMessages([FromQuery] GetMessagesByUserQuery query)
        {
            var getMessages = await _mediator.Send(query);
            return Ok(getMessages);
        }


        [HttpGet("GetAllChatUser")]
        public IActionResult GetAllChatUser()
        {
            return null;
        }

        [HttpGet("GetOnlineUsers")]
        public IActionResult GetOnlineUsers()
        {
            return null;
        }



    }
}
