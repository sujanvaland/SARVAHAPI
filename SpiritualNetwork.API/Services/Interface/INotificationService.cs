﻿using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface INotificationService
    {
        public Task SendEmailNotification(string emailType, User user);
        public Task<JsonResponse> SaveNotification(NotificationRes Res);
        public Task<JsonResponse> UserNotification(int userId, int PageNo, int Size);

    }
}
