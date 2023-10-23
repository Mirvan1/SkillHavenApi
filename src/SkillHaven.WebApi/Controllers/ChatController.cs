using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared.Chat;

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
        public async Task<IActionResult> GetAllChatUser([FromQuery] GetAllChatUserQuery query)
        {
            var getAllChatUser = await _mediator.Send(query);
            return Ok(getAllChatUser);
        }

        [HttpGet("GetOnlineUsers")]
        public async Task<IActionResult> GetOnlineUsers([FromQuery] GetOnlineUsersQuery query)
        {
            var getOnlineUsers = await _mediator.Send(query);
            return Ok(getOnlineUsers);
        }


        [HttpGet("GetChatUser/{UserId}")]
        public async Task<IActionResult> GetChatUser(int UserId)
        {
            GetChatUserQuery query = new() { UserId=UserId };
            var getChatUser = await _mediator.Send(query);
            return Ok(getChatUser);
        }


    }
}
