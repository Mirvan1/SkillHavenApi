using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Chat;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Chat.Message.Queries
{
    public class GetMessagesByUserQueryHandler : IRequestHandler<GetMessagesByUserQuery, PaginatedResult<GetMessagesByUserDto>>
    {
        public readonly IMessageRepository _messageRepository;
        public readonly IChatUserRepository _chatUserRepository;
        public readonly IUserService _userService;
        public readonly IUserConnectionRepository _chatUserConnectionRepository;
        public readonly IStringLocalizer _localizer;


        public GetMessagesByUserQueryHandler(IMessageRepository messageRepository, IChatUserRepository chatUserRepository, IUserService userService, IUserConnectionRepository chatUserConnectionRepository)
        {
            _messageRepository=messageRepository;
            _chatUserRepository=chatUserRepository;
            _userService=userService;
            _chatUserConnectionRepository=chatUserConnectionRepository;
            _localizer=new Localizer();

        }



        public Task<PaginatedResult<GetMessagesByUserDto>> Handle(GetMessagesByUserQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);


            var getUser = _userService.GetUser();

            if (getUser is null) throw new UserVerifyException("Sonething wrong in authorize");

            var getChatUser = _chatUserRepository.getByUserId(getUser.UserId);

            if (getUser is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Chat User History"].Value);

            //var getSenderConnection = _chatUserConnectionRepository.GetByUserId(getChatUser.Id);

            //if (getSenderConnection is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User Chat Hub"].Value);

            var getReceviderChatUser= _chatUserRepository.getByUserId(request.ReceiverUserId);

            if (getReceviderChatUser is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User Chat Hub"].Value);



            Expression<Func<SkillHaven.Domain.Entities.Message, bool>> filterExpression = null;
            Func<IQueryable<SkillHaven.Domain.Entities.Message>, IOrderedQueryable<SkillHaven.Domain.Entities.Message>> orderByExpression = null;

            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.SenderId==getChatUser.Id && entity.ReceiverId== getReceviderChatUser.Id;
            }

            var includeProperties = new Expression<Func<SkillHaven.Domain.Entities.Message, object>>[]
            {
                //e => e.Sender,
                //e=>e.Receiver
            };


           // var getMessages = _messageRepository.GetMessagesBySender(getChatUser.Id);
            var getMessages = _messageRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, includeProperties);

            if (getMessages is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Messages"].Value);

            PaginatedResult<GetMessagesByUserDto> result = new()
            {
                TotalCount=getMessages.TotalCount,
                TotalPages=getMessages.TotalPages,
            };

          //List<GetMessagesByUserDto> getMessagesDtoList = new();
            //foreach (var message in getMessages)
            //{
            //    var getMessageByUser = new GetMessagesByUserDto()
            //    {
            //        MessageId=message.MessageId,
            //        SenderChatId=message.SenderId,
            //        SenderUserId=message.Sender.UserId,
            //        ReceiverChatId=message.ReceiverId,
            //        ReceiverUserId=message.Receiver.UserId,
            //        Content=message.Content,
            //        Timestamp=message.Timestamp,
            //        MessageType=message.MessageType,
            //        SeenStatus=message.SeenStatus,
            //        SenderProfilePicture=message.Sender.ProfilePicture,
            //        ReceiverProfilePicture=message.Receiver.ProfilePicture,
            //        SenderStatus=message.Sender.Status,
            //        ReceiverStatus=message.Receiver.Status
            //    };
            //    getMessagesDtoList.Add(getMessageByUser);
            //}

            List<GetMessagesByUserDto> getMessagesDtoList = getMessages.Data.Select(message => new GetMessagesByUserDto
            {
                MessageId = message.MessageId,
                SenderChatId = message.SenderId,
                SenderUserId = _chatUserRepository.GetById(message.SenderId).UserId,
                ReceiverChatId = message.ReceiverId,
                ReceiverUserId =_chatUserRepository.GetById(message.ReceiverId).UserId,//change this due to performance
                Content = message.Content,
                Timestamp = message.Timestamp,
                MessageType = message?.MessageType,
                SeenStatus = message?.SeenStatus,
                SenderProfilePicture = _chatUserRepository.GetById(message.SenderId)?.ProfilePicture,
                ReceiverProfilePicture = _chatUserRepository.GetById(message.ReceiverId)?.ProfilePicture,
                SenderStatus = _chatUserRepository.GetById(message.SenderId)?.Status,
                ReceiverStatus =_chatUserRepository.GetById(message.ReceiverId)?.Status
            }).ToList();
            result.Data=getMessagesDtoList;

            return Task.FromResult(result);
           // throw new NotImplementedException();
        }
    }
}
