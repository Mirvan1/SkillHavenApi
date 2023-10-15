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
    public class BlogController : BaseController
    {
        public BlogController(IMediator mediator) : base(mediator)
        {

        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetBlogsQuery query)
        {
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {
            var query = new GetBlogQuery() { Id=Id };
            var getAll = await _mediator.Send(query);

            return Ok(getAll);
        }



        [HttpPost]
        public async Task<IActionResult> Create( CreateUserCommand command)
        {
            var create = await _mediator.Send(command);

            return Ok(create);
        }


        [HttpPut]
        public async Task<IActionResult> Create(UpdateBlogCommand command)
        {
            var update = await _mediator.Send(command);

            return Ok(update);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteBlogCommand command)
        {
            var delete = await _mediator.Send(command);

            return Ok(delete);
        }


        [HttpPut("/vote")]
        public async Task<IActionResult> VoteBlog(VoteBlogCommand command)
        {
            var update = await _mediator.Send(command);

            return Ok(update);
        }


        [HttpGet("/comments/{blogId}")]
        public async Task<IActionResult> GetBlogComments(int blogId, [FromQuery] int Page, 
            [FromQuery]int PageSize, [FromQuery] bool OrderBy, [FromQuery] string OrderByParameter)
        {
            var query = new BlogByCommentsQuery()
            {
                BlogId=blogId,
                Page=Page,
                PageSize=PageSize,
                OrderBy=OrderBy,
                OrderByPropertname=OrderByParameter
            };
            var comments = await _mediator.Send(query);
            return Ok(comments);
        }


        [HttpPost("/commment/create")]
        public async Task<IActionResult> CreateBlogComment([FromBody] CreateBlogCommentCommand command)
        {
            var create = await _mediator.Send(command);
            return Ok(create);
        }


        [HttpDelete("comment/{commentId}")]
        public async Task<IActionResult> DeleteBlogComment(int commentId)
        {
            var query = new DeleteBlogCommentCommand() { BlogCommentId=commentId };
            var deleted = await _mediator.Send(query);
            return Ok(deleted);
        }
    }
}
