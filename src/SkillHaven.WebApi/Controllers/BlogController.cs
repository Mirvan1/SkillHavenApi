﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillHaven.Shared;

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


    }
}