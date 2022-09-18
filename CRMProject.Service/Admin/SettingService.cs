using CRMProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using CRMProject.ViewModels.Admin;
using CRMProject.ViewModels.Common;

namespace CRMProject.Service.Admin
{
    public class SettingService
    {
        private readonly CrmDbTestEntities _context;
        public SettingService(CrmDbTestEntities context)
        {
            _context = context;
        }
        public bool Login(LoginViewModel login)
        {
            return _context.Users.Any(x => x.Password == login.Password && x.UserName == login.UserName);
        }
        public Users Login2(LoginViewModel login)
        {
            var model= _context.Users.FirstOrDefault(x => x.Password == login.Password && x.UserName == login.UserName);
            return model;
        }

        public ServiceCallResult Register(RegisterViewModel model)
        {
            var callresult = new ServiceCallResult() { Success = false };
            bool name = _context.Users.Any(x => x.UserName == model.Name);
            if (name)
            {
                callresult.ErrorMessages.Add("Bu kullanıcı zaten kayıtlı!");
                return callresult;
            }

            var users = new Users()
            {
                UserName = model.Name,
                Password = model.Password
            };
            _context.Users.Add(users);
            using (var dbtransaction=_context.Database.BeginTransaction())
            {
                try
                {
                 _context.SaveChanges();
                dbtransaction.Commit();
                callresult.SuccessMessages.Add("Kullanıcı Kayıt Edildi");
                callresult.Success = true;
                return callresult;
                }
                catch (Exception exc)
                {
                    callresult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callresult;
                }
                
                   
            }
        }
    }
}
