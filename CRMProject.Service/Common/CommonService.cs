using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMProject.Data;
using CRMProject.ViewModels.Admin;

namespace CRMProject.Service.Common
{
    public class CommonService
    {
        private readonly CrmDbTestEntities _context;
        public CommonService(CrmDbTestEntities context)
        {
            _context = context;
        }
        public async Task<CurrentUserModel> GetCurrentUserModelAsync(long userId)
        {
            var userQuery = from u in _context.Users
                where
                    u.Id == userId
                select new CurrentUserModel
                {
                    UserId = u.Id,
                   UserName = u.UserName,
                    Role = u.Roles.Role,
                    AuthCodes=new List<string>(){u.Roles.Role}
                };

            return await userQuery.SingleOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
