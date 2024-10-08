﻿using BusinessObject.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.LicenceRepository
{
    public interface ILicenceRepository
    {
        void InsertLicence(Licence licence);
        Task<Licence> GetLicenceByUserId(Guid loggedInUserId);
        void UpdateLicence(Licence existingLicence);
        Task<bool> IsUserLicence(Guid userId);
    }
}
