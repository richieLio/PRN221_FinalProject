using BusinessObject.Object;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class LicenceDAO
    {
        private static LicenceDAO instance = null;
        private static readonly object instanceLock = new object();
        private LicenceDAO() { }
        public static LicenceDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new LicenceDAO();
                    }
                    return instance;
                }
            }
        }

        public void InsertLicence(Licence licence)
        {
            using var context = new RmsContext();
            context.Licences.Add(licence);
            context.SaveChanges();
        }

        public void UpdateLicence(Licence existingLicence)
        {
            using var context = new RmsContext();
            context.Licences.Update(existingLicence);
            context.SaveChanges();
        }

        public async Task<Licence> GetLicenceByUserId(Guid loggedInUserId)
        {
            using var context = new RmsContext();
            return await context.Licences.Where(l => l.UserId == loggedInUserId).FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserLicence(Guid userId)
        {
            using var context = new RmsContext();

            // Kiểm tra xem người dùng có vai trò staff không
            var isStaff = await context.Users.Where(u => u.Id == userId && u.Role == UserEnum.STAFF).FirstOrDefaultAsync();
            var isOwner = await context.Users.Where(u => u.Id == userId && u.Role == UserEnum.OWNER).FirstOrDefaultAsync();
            if (isStaff != null)
            {
                // Nếu người dùng là staff, lấy thông tin giấy phép của chủ sở hữu (owner)
                var owner = await context.Users.FirstOrDefaultAsync(l => l.Id == isStaff.OwnerId);
                var licence = await context.Licences.Where(l => l.UserId == owner.Id && l.IsLicence == true).FirstOrDefaultAsync();
                if (licence != null)
                {
                    return true;
                }
                else
                {
                    return false;
                };
            } else if (isOwner != null)
            {
                var licence = await context.Licences.Where(l => l.UserId == isOwner.Id && l.IsLicence == true).FirstOrDefaultAsync();
                if (licence != null)
                {
                    return true;
                }
                else
                {
                    return false;
                };
            }
            // Trường hợp còn lại, trả về false
            return false;
        }

    }
}
