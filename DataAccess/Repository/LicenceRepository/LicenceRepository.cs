using BusinessObject.Object;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.LicenceRepository
{
    public class LicenceRepository : ILicenceRepository
    {
        public Task<Licence> GetLicenceByUserId(Guid loggedInUserId) => LicenceDAO.Instance.GetLicenceByUserId(loggedInUserId);

        public void InsertLicence(Licence licence) => LicenceDAO.Instance.InsertLicence(licence);

        public Task<bool> IsUserLicence(Guid userId) => LicenceDAO.Instance.IsUserLicence(userId);

        public void UpdateLicence(Licence existingLicence) => LicenceDAO.Instance.UpdateLicence(existingLicence);
    }
}
