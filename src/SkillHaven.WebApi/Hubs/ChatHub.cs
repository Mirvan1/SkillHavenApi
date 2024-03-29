﻿using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Repositories;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using System.Linq.Expressions;
using System.Security.Claims;

namespace SkillHaven.WebApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserConnectionRepository _userConnectionRepo;
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ILoggerService<ChatHub> _logger;


        public ChatHub(IUserConnectionRepository userConnectionRepo, IMessageRepository messageRepository, IChatUserRepository chatUserRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IUserService userService, ILoggerService<ChatHub> logger)
        {
            _userConnectionRepo = userConnectionRepo;
            _messageRepository = messageRepository;
            _chatUserRepository = chatUserRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _logger = logger;
        }

        public async Task SendMessageToAll(string messageContent)
        {
            //var userId = int.Parse(Context.User.Identity.Em);
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var currentUser = _userRepository.GetByEmail(email);
           
            var getSenderChatUser = _chatUserRepository.getByUserId(currentUser.UserId);
            if (getSenderChatUser is null) throw new HubConnectionException(" sender not found");

            var chatUsers = _userRepository.GetAll();

            if (chatUsers is null) throw new HubConnectionException("Users cannot found");

            foreach (var user in chatUsers)
            {
                var getReceiverChatUser = _chatUserRepository.getByUserId(currentUser.UserId);

                if (getReceiverChatUser is null) throw new HubConnectionException("one of receiver not found");

                var message = new Message
                {
                    SenderId = getSenderChatUser.Id,
                    Content = messageContent,
                    Timestamp = DateTime.Now,
                    ReceiverId = getReceiverChatUser.Id,
                    MessageType = MessageType.All.ToString(),
                };
                _messageRepository.Add(message);
            }

            _messageRepository.SaveChanges();
            await Clients.All.SendAsync("ReceiveMessageToAll", currentUser.UserId, messageContent);
        }

        public async Task SendMessageToClient(int receiverId, string messageContent)
        {
            //   var userId = int.Parse(Context.User.Identity.Name);
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var currentUser = _userRepository.GetByEmail(email);

            var getSenderChatUser = _chatUserRepository.getByUserId(currentUser.UserId);
            if (getSenderChatUser is null) throw new HubConnectionException(" sender not found");

            var getSenderConnectionInfo = _userConnectionRepo.GetByChatUserId(getSenderChatUser.Id);
            if (getSenderChatUser is null) throw new HubConnectionException(" sender not found");



            var getReceiverChatUser = _chatUserRepository.getByUserId(receiverId);
            if (getReceiverChatUser is null) throw new HubConnectionException(" receiver not found");

            var messageClient = new Message
            {
                SenderId = getSenderChatUser.Id,
                Content = messageContent,
                Timestamp = DateTime.Now,
                ReceiverId = getReceiverChatUser.Id,
                MessageType = MessageType.Client.ToString(),
            };

            _messageRepository.Add(messageClient);
            _messageRepository.SaveChanges();

            var receiver = _userConnectionRepo.GetByChatUserId(getReceiverChatUser.Id);//useridye gore bul
            if (receiver is null)
            {
                throw new HubConnectionException("Receiver can not find");
            }

            await Clients.Client(getSenderConnectionInfo.ConnectionId).SendAsync($"SenderMessageToClient", receiverId, messageContent);

            await Clients.Client(receiver.ConnectionId).SendAsync($"ReceiveMessageToClient", currentUser.UserId.ToString(), messageContent);
            await Clients.Client(getSenderConnectionInfo.ConnectionId).SendAsync($"ReceiveMessageToClient", receiverId, messageContent);
           // _logger.LogError($"Hub Connection: Sender  :{currentUser.FirstName} -- Receiver:{receiverId}  -- Message:{messageContent}");
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToGroup(string user, string message, string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessageToAll", user, message);
        }

        public async Task SendMessageToSuperVisors(string messageContent, int userId = 1)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var currentUser = _userRepository.GetByEmail(email);
            var userAdmin = _userRepository.GetById(currentUser.UserId);

            if (userAdmin.Role != Role.Admin.ToString()) throw new HubConnectionException("Only Admin use this feature");
            var getSenderChatUser = _chatUserRepository.getByUserId(currentUser.UserId);
            if (getSenderChatUser is null) throw new HubConnectionException("Chat user not found..");

            var supervisors = _userRepository.GetAllSupervisors();
            IEnumerable<string> supervisorConnections = GetSupervisorConnectionIds(supervisors);

            foreach (var user in supervisors)
            {
                var getReceiverChatUser = _chatUserRepository.getByUserId(currentUser.UserId);

                if (getReceiverChatUser is null) throw new HubConnectionException("one of receiver not found");

                var message = new Message
                {
                    SenderId = getSenderChatUser.Id,
                    Content = messageContent,
                    Timestamp = DateTime.Now,
                    ReceiverId = getReceiverChatUser.Id,
                    MessageType = MessageType.AllSupervisors.ToString(),
                };
                _messageRepository.Add(message);
            }

            _messageRepository.SaveChanges();

            await Clients.AllExcept(supervisorConnections).SendAsync("ReceiveMessageToSuperVisors", userAdmin.UserId, messageContent);
        }

        public async Task SendMessageToConsultants(string messageContent, int userId = 1)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var currentUser = _userRepository.GetByEmail(email);
            var userAdmin = _userRepository.GetById(currentUser.UserId);

            if (userAdmin.Role != Role.Admin.ToString()) throw new HubConnectionException("Only Admin use this feature");

            var supervisors = _userRepository.GetAllConsultants();
            var getSenderChatUser = _chatUserRepository.getByUserId(currentUser.UserId);
            if (getSenderChatUser is null) throw new HubConnectionException("Chat user not found..");

            IEnumerable<string> supervisorConnections = GetConsultantConnectionIds(supervisors);

            foreach (var user in supervisors)
            {
                var getReceiverChatUser = _chatUserRepository.getByUserId(currentUser.UserId);

                if (getReceiverChatUser is null) throw new HubConnectionException("one of receiver not found");

                var message = new Message
                {
                    SenderId = getSenderChatUser.Id,
                    Content = messageContent,
                    Timestamp = DateTime.Now,
                    ReceiverId = getReceiverChatUser.Id,
                    MessageType = MessageType.AllConsultants.ToString(),
                };
                _messageRepository.Add(message);
            }

            _messageRepository.SaveChanges();

            await Clients.AllExcept(supervisorConnections).SendAsync("ReceiveMessageToConsultants", userAdmin.UserId, messageContent);
        }

        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessageToCaller", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                //var userId = int.Parse(Context.User.Claims.FirstOrDefault(x => x.Type==ClaimTypes.Email).Value;
                var user = _userService.GetUser();

                string EmailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
                var email = Context.User.Claims.FirstOrDefault(x => x.Type == EmailClaim).Value;
                var currentUser = _userRepository.GetByEmail(email);
                var connectionId = Context.ConnectionId;


                var checkChatUser = _chatUserRepository.getByUserId(currentUser.UserId);

                if (checkChatUser is null)
                {
                    var newChatUser = new ChatUser
                    {
                        UserId = currentUser.UserId,
                        LastSeen = DateTime.Now,
                        Status = ChatUserStatus.Online.ToString()
                    };
                    _chatUserRepository.Add(newChatUser);
                    _chatUserRepository.SaveChanges();
                }
                else
                {

                    if (checkChatUser.Status==ChatUserStatus.Offline.ToString())
                    {
                        checkChatUser.LastSeen=DateTime.Now;    
                        checkChatUser.Status=ChatUserStatus.Online.ToString();
                        _chatUserRepository.Update(checkChatUser);
                        _chatUserRepository.SaveChanges();
                    }
                }


                bool checkConnections = _userConnectionRepo.CheckUserConnected(currentUser.UserId, connectionId);

                if (checkConnections)
                {
                    throw new HubConnectionException("User already connected");
                }
                else
                {
                    var getChatUser = _chatUserRepository.getByUserId(currentUser.UserId);
                    var newConnection = new ChatUserConnection
                    {
                        ChatUserId = getChatUser.Id,
                        ConnectionId = connectionId,
                        ConnectedTime = DateTime.Now
                    };
                    _userConnectionRepo.Add(newConnection);
                    _userConnectionRepo.SaveChanges();
                }
                await base.OnConnectedAsync();
            }

            catch (Exception e)
            {
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userConnection = _userConnectionRepo.GetByConnnectionId(connectionId);

            if (userConnection is null) throw new HubConnectionException("Relate connectionId do not found");

            userConnection.ChatUser.Status = ChatUserStatus.Offline.ToString();
            _chatUserRepository.Update(userConnection.ChatUser);
            _chatUserRepository.SaveChanges();


            _userConnectionRepo.RemoveByConnectionId(connectionId);
            _userConnectionRepo.SaveChanges();

            await base.OnDisconnectedAsync(exception);
        }


        public async Task OnDisconnectedNoException()
        {
            var connectionId = Context.ConnectionId;
            var userConnection = _userConnectionRepo.GetByConnnectionId(connectionId);

            if (userConnection is null) throw new HubConnectionException("Relate connectionId do not found");

            userConnection.ChatUser.Status = ChatUserStatus.Offline.ToString();
            _chatUserRepository.Update(userConnection.ChatUser);
            _chatUserRepository.SaveChanges();


            _userConnectionRepo.RemoveByConnectionId(connectionId);
            _userConnectionRepo.SaveChanges();

            await base.OnDisconnectedAsync(new HubException("The connection is terminated"));
        }

        public async Task Ping()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("Pong");
        }

        public async IAsyncEnumerable<int> StreamData(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return i;
                await Task.Delay(100, cancellationToken);
            }
        }

        public int GetUserIdFromConnectionId(string connectionId)
        {
            return _userConnectionRepo.GetByConnnectionId(connectionId).ChatUserId;
        }

        public string GetConnectionIdFromUserId(int userId)
        {
            return _userConnectionRepo.GetByUserId(userId).ConnectionId;
        }


        private IEnumerable<string> GetSupervisorConnectionIds(List<User> supervisors)
        {
            foreach (var user in supervisors)
            {
                var chatUsers = _userConnectionRepo.GetByUserId(user.UserId);
                yield return chatUsers.ConnectionId;
            }
        }


        private IEnumerable<string> GetConsultantConnectionIds(List<User> consultants)
        {
            foreach (var user in consultants)
            {
                var chatUsers = _userConnectionRepo.GetByUserId(user.UserId);
                yield return chatUsers.ConnectionId;
            }
        }
        //private static readonly Dictionary<string, string> userIdToConnectionMap = new Dictionary<string, string>();
        //private static readonly Dictionary<string, string> connectionToUserIdMap = new Dictionary<string, string>();

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        //public async Task SendMessageToClient(string user, string message, string connectionId)
        //{
        //    await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message);
        //}

        //public async Task JoinGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //}

        //public async Task SendMessageToGroup(string user, string message, string groupName)
        //{
        //    await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        //}

        //public async Task SendMessageToOthers(string user, string message, List<string> excludedConnectionIds)
        //{
        //    await Clients.AllExcept(excludedConnectionIds).SendAsync("ReceiveMessage", user, message);
        //}
        //public async Task SendMessageToCaller(string user, string message)
        //{
        //    await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        //}





        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.User.Identity.Name;  // Replace this with how you get your user identifier
        //    var connectionId = Context.ConnectionId;
        //    userIdToConnectionMap[userId] = connectionId;
        //    connectionToUserIdMap[connectionId] = userId;
        //    // Custom logic here, e.g., adding the connection to a list
        //    await base.OnConnectedAsync();
        //}


        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var connectionId = Context.ConnectionId;
        //    if (connectionToUserIdMap.TryGetValue(connectionId, out var userId))
        //    {
        //        userIdToConnectionMap.Remove(userId);
        //        connectionToUserIdMap.Remove(connectionId);
        //    }
        //    await base.OnDisconnectedAsync(exception);
        //}


        //public async IAsyncEnumerable<int> StreamData(CancellationToken cancellationToken)
        //{
        //    for (var i = 0; i < 100; i++)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();
        //        yield return i;
        //        await Task.Delay(100, cancellationToken);
        //    }
        //}


        //public string GetUserIdFromConnectionId(string connectionId)
        //{
        //    if (connectionToUserIdMap.TryGetValue(connectionId, out var userId))
        //    {
        //        return userId;
        //    }
        //    return null;
        //}
        //public string GetConnectionIdFromUserId(string userId)
        //{
        //    if (userIdToConnectionMap.TryGetValue(userId, out var connectionId))
        //    {
        //        return connectionId;
        //    }
        //    return null;
        //}
    }


    public enum MessageType
    {
        All = 1,
        AllSupervisors = 2,
        Client = 3,
        AllConsultants = 4
    }

    public enum ChatUserStatus
    {
        Online = 1,
        Offline = 2
    }
}
