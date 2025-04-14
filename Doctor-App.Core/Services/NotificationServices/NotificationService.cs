using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.NotificationServices
{
    /*public class NotificationService : INotificationService
    {
        private readonly DoctorAppDbContext _dbContext;

        public NotificationService (DoctorAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateNotificationAsync(Guid userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.isRead = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }*/
}
