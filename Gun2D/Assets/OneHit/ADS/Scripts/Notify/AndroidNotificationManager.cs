using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneHit.Notification
{
    public class AndroidNotificationManager
    {

        /*public void Initialize()
        {
            CreateChannel();
        }

        public void ScheduleNotification(Notification notification, DateTime fireTime, int id)
        {
            var androidNoti = new AndroidNotification();
            androidNoti.Title = notification.title;
            androidNoti.Text = notification.GetMessageWithIcon();
            androidNoti.FireTime = fireTime;

            Debug.LogWarning("Fire Noti At: " + androidNoti.FireTime + "\nTitle: " + androidNoti.Title + "\nMessage: " + androidNoti.Text);
            AndroidNotificationCenter.SendNotificationWithExplicitID(androidNoti, Application.identifier, id);
        }

        public void CancelAllNotifications()
        {
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }

        private void CreateChannel()
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = Application.identifier,
                Name = Application.productName,
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }*/
    }
}
