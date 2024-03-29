﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.User;
using SkillHaven.Shared.User.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillHaven.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private IUserRepository _userRepository;
        private readonly IMediator _mediator;


        public UserController(IMediator mediator, IMapper mapper, IUserRepository userRepository) : base(mediator)
        {
            _mapper = mapper;
            _userRepository=userRepository;
            _mediator = mediator;

        }

 
        [HttpGet("get-logged-user")]
        public async Task<IActionResult> GetUser()
        {
            var query = new GetLoggedUserQuery {  };
            var userDto = await _mediator.Send(query);

            return Ok(userDto);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var query = new GetUserQuery { UserId = userId };
            var userDto = await _mediator.Send(query);

            return Ok(userDto);
        }



        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> UserRegistration(RegisterUserCommand command)
        {

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var result = await _mediator.Send(new DeleteUserCommand() { UserId=UserId });
            return Ok(result);
        }


        [HttpPost("login"),AllowAnonymous]
        public async Task<IActionResult> UserLogin(LoginUserCommand command)
        {

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("verify-user"), AllowAnonymous]
        public async Task<IActionResult> VerifyUser(VerifyUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("forgot-password"), AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("reset-password"), AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("mail-checker"), AllowAnonymous]
        public async Task<IActionResult> MailUserChecker(MailUserCheckerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpGet("get-captcha-key"), AllowAnonymous]
        public async Task<IActionResult> GetCaptchaKey()
        {
            var result = await _mediator.Send(new GetCaptchaUIKeyQuery { });
            return Ok(result);
        }


        [HttpPost("auth"),AllowAnonymous]
        public async Task<IActionResult> Auth(AuthUserCommand command)//add controll by ip
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
