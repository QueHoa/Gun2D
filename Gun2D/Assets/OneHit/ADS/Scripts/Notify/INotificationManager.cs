using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneHit.Notification
{
    public interface INotificationManager
    {
        void Initialize();
        void ScheduleNotification(Notification notification, DateTime fireTime, int id);
        void CancelAllNotifications();
    }
}