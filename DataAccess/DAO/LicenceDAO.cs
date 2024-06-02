using BusinessObject.Object;
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
    }
}
