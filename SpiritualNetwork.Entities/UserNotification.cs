﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserNotification : BaseEntity
    {
        public int NotificationId { get;set; }
        public int UserId { get;set; }
    }
}
