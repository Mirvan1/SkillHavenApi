using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.User;

namespace SkillHaven.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : BaseController
    {
        public BlogController(IMediator mediator) : base(mediator)
        {

        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]  GetBlogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {;
            var result = await _mediator.Send(new GetBlogQuery() { Id = Id });
            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> Create( CreateBlogCommand  command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Create(UpdateBlogCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteBlogCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPut("vote")]
        public async Task<IActionResult> VoteBlog(VoteBlogCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("comments")]
        public async Task<IActionResult> GetBlogComments(  BlogByCommentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost("commment/create")]
        public async Task<IActionResult> CreateBlogComment([FromBody] CreateBlogCommentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpDelete("comment/{commentId}")]
        public async Task<IActionResult> DeleteBlogComment(int commentId)
        {
            var result = await _mediator.Send(new DeleteBlogCommentCommand() { BlogCommentId = commentId });
            return Ok(result);
        }


        [HttpGet("topics")]
        public async Task<IActionResult> GetBlogTopics([FromQuery]GetBlogTopicsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


    }
}
