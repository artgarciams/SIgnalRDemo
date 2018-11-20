using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRDBDemo.Repository
{
    public class NotificationRespository
    {
        public void AddNotification(string Text, string UserName)
        {
            using (NotificationEntities ent = new NotificationEntities())
            {
                NotificationList obj = new NotificationList();
                obj.Text = Text;
                obj.UserID = UserName;
                obj.DateStamp = DateTime.Now.ToUniversalTime();
                ent.NotificationLists.Add(obj);
                ent.SaveChanges();
            }
        }

        public List<NotificationList> GetNotifications(string userName)
        {

            using (NotificationEntities ent = new NotificationEntities())
            {
                return ent.NotificationLists.Where(e => e.UserID == userName).ToList();
            }
        }

        public List<NotificationList> GetLatestNotifications(DateTime dt)
        {

            using (NotificationEntities ent = new NotificationEntities())
            {
                if (dt == DateTime.MinValue)
                {
                    return ent.NotificationLists.ToList();
                }
                else
                {
                    DateTime dtUTC = dt.ToUniversalTime();
                    return ent.NotificationLists.Where(e => e.DateStamp > dtUTC).ToList();
                }
            }
        }
    }
}