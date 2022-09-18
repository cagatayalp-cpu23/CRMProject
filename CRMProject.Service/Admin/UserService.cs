using CRMProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMProject.ViewModels.Admin;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using CRMProject.ViewModels.Common;
using LinqKit;

namespace CRMProject.Service.Admin
{

    public class UserService
    {
        private readonly CrmDbTestEntities _context;

        public UserService(CrmDbTestEntities context)
        {
            _context = context;
        }
        public List<NameSelectModel> GetUsers(int id, string role)
        {
            if (role == "IT")
            {
                //foreach (UserViewModel item in _context.Users)
                //{
                //    item.Name=
                //}
                return (from b in _context.Users.Where(x => x.Roles.Role != "Manager").Where(x => x.UserName != null)
                        select new NameSelectModel { Name = b.UserName }).ToList();
            }
            else if (role == "Manager")
            {
                return (from b in _context.Users.Where(x => x.UserName != null)
                        select new NameSelectModel { Name = b.UserName }).ToList();
            }

            return null;

        }

        public IQueryable<UserListViewModel> GetUserListIQueryable(UserSearchViewModel model)
        {
            var predicate = PredicateBuilder.New<Users>(true);
            if (!string.IsNullOrEmpty(model.Name))
            {
                predicate.And(x => x.UserName.Contains(model.Name));
            }
            return _getUserListIQueryable(predicate);
        }
        public UserListViewModel GetOneUserListIQueryable(int id)
        {
            var predicate = PredicateBuilder.New<Users>(true);
            predicate.And(x => x.Id == id);
            return _getUserListIQueryable(predicate).FirstOrDefault();
        }
        public UserEditViewModel GetOneUserEditListIQueryable(int id)
        {
            var predicate = PredicateBuilder.New<Users>(true);
            predicate.And(x => x.Id == id);
            return _getUserListEditIQueryable(predicate).FirstOrDefault();
        }

        private IQueryable<UserListViewModel> _getUserListIQueryable(Expression<Func<Data.Users, bool>> expr)
        {
            return from b in _context.Users.Where(expr)
                   select new UserListViewModel()
                   {
                       Id = b.Id,
                       Name = b.UserName,
                       Password = b.Password,
                       Roles = (from x in b.UserRole
                                select new RoleEditViewModel()
                                {
                                    Name = new List<string> { x.Roles.Role }


                                }),
                       Roles2 = (from x in b.UserRole
                                 select new UserAddViewModel()
                                 {

                                     Role = new List<string> { x.Roles.Role }

                                 })

                   };
        }
        private IQueryable<UserEditViewModel> _getUserListEditIQueryable(Expression<Func<Data.Users, bool>> expr)
        {
            return from b in _context.Users.Where(expr)
                   select new UserEditViewModel()
                   {
                       Id = b.Id,
                       Name = b.UserName,
                       Password = b.Password,
                       Role = new List<string> { b.Roles.Role }
                   };
        }
        public ServiceCallResult AddUser(UserAddViewModel model)
        {

            var callresult = new ServiceCallResult() { Success = false };
             //_context.Users.Select(x=>new
             //{
             //    u=x,
             //    tu=x.TaskUser.ToList(),
             //    ur=x.UserRole,

             //})
            bool isuserexist = _context.Users.Any(x => x.UserName == model.Name);
            if (!isuserexist)
            {
                foreach (var item in model.Roles)
                {
                    if (item.IsSelected == true)
                    {
                        var role = _context.Roles.FirstOrDefault(x => x.Role == item.Name);

                    }
                }





                var user = new Users()
                {
                    UserName = model.Name,
                    Password = model.Password,


                };
                _context.Users.Add(user);
                // foreach (var item in model.Role)
                // {

                foreach (var item2 in model.Roles)
                {
                    if (item2.IsSelected == true)
                    {
                        var role = _context.Roles.FirstOrDefault(x => x.Role == item2.Name);
                        _context.UserRole.Add(
                            new UserRole()
                            {
                                roleId = role.Id,
                                userId = user.Id,
                            });
                    }
                }

                // }


                using (var dbtransaction = _context.Database.BeginTransaction())
                {
                    _context.SaveChanges();
                    dbtransaction.Commit();
                    callresult.Success = true;
                    callresult.Item = user.Id;
                    return callresult;
                }
            }
            else
            {
                callresult.ErrorMessages.Add("Bu isimde kullanıcı zaten var");
                return callresult;
            }



        }
        //yapıldı false sa silinemez yapıldı trueysa silinir tasksette varsa ve yapıldı trueysa silinir tasksette vaarsa ve yapıldı false sa silinmez
        public ServiceCallResult DeleteUser(int id)
        {
            int k = 0;
            var callResult = new ServiceCallResult() { Success = false };
            var user = _context.TaskUser.Where(x => x.userId == id).ToList();
            //foreach (var item in user)
            //{
            //    item.TaskSet.IsSelected
            //}
            //  bool yapildimi = (bool)user.TaskSet.IsSelected;
            bool silinecekuser = _context.TaskUser.Any(x => x.userId == id);

            if (silinecekuser)
            {
                foreach (var item in user)
                {
                    if (!(bool)item.TaskSet.IsSelected)
                    {
                        callResult.ErrorMessages.Add("Bu kullanıcının yapılacak taskı var");
                        return callResult;
                    }
                }




            }
            var model = _context.Users.FirstOrDefault(x => x.Id == id);
            var model2 = _context.UserRole.Where(x => x.userId == id).ToList();
            var model3 = _context.TaskUser.Where(x => x.userId == id).ToList();
            _context.TaskUser.RemoveRange(model3);
            _context.UserRole.RemoveRange(model2);
            _context.Users.Remove(model);
            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                dbtransaction.Commit();
                _context.SaveChanges();
                callResult.Success = true;
                return callResult;
            }


        }

        public UserEditViewModel EditUserForm(int id)
        {
            //   var roles = _context.Roles.ToList();

            //   var role = roles.FirstOrDefault().Role;
            var model = _context.Users.Find(id);
            var userroles = _context.UserRole.Where(x => x.userId == id).ToList();
            // var trueroles = _context.Roles.Where(x => x.IsSelected == true).ToList();


            UserEditViewModel user = new UserEditViewModel()
            {
                Id = model.Id,
                Name = model.UserName,
                // Role = new List<string>{string.Join("",userroles.SelectMany(x=>x.Roles.Role.ToString()))},
                // roleId = model.Roles.Id,
                Password = model.Password,
                Roles = (from db in _context.Roles
                         select new RoleViewModel()
                         {
                             Name = db.Role,
                             Id = db.Id,

                         }).ToList()

            };
            foreach (var item in user.Roles)
            {
                foreach (var item2 in userroles)
                {
                    if (item2.Roles.Role == item.Name)
                    {
                        item.IsSelected = true;
                    }
                }

            }

            //foreach (var item in user.Roles)
            //{
            //    foreach (var item2 in trueroles)
            //    {
            //        if (item2.Role==item.Name)
            //        {
            //            item.IsSelected = true;
            //        }
            //    }
            //}

            return user;

        }

        public ServiceCallResult EditUser(UserEditViewModel model)
        {
            var user = _context.Users.Find(model.Id);
            // var role = _context.Roles.Where(x=>x.Role==model.Name&&model.Roles.Any(m=>m.IsSelected==true)).ToList();
            // foreach (var item in role)
            // {
            //    item.IsSelected = true;
            // }
            foreach (var item2 in model.Roles)
            {
                if (item2.IsSelected)
                {

                    var rolename = _context.Roles.FirstOrDefault(x => x.Role == item2.Name);
                    var username = _context.Users.FirstOrDefault(x => x.Roles.Role == item2.Name);
                    _context.UserRole.Add(new UserRole()
                    {
                        roleId = rolename.Id,
                        userId = model.Id
                    });

                }
            }
            var callresult = new ServiceCallResult() { Success = false };
            var finduser = _context.Users.Find(model.Id);
            bool ispasswordtrue = finduser.Password == model.OldPassword;
            if (!ispasswordtrue)
            {
                callresult.ErrorMessages.Add("Yanlış eski şifre");
                return callresult;
            }

            var user2 = _context.Users.Where(x => x.Id == model.Id).ToList();
            user.UserName = model.Name;
            user.Password = model.Password;
            _context.Users.Add(user);

            //  user2.Add(new Users()
            //  {
            //      RoleId = model.roleId,
            //      Roles = model.Role
            //  });
            // user.Roles.Role = model.Role;

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                dbtransaction.Commit();
                _context.SaveChanges();
                callresult.Success = true;
                callresult.Item = user.Id;
                return callresult;
            }

        }

        public UserAddViewModel UserForm()
        {
            UserAddViewModel user = new UserAddViewModel()
            {
                Roles = (from db in _context.Roles
                         select new RoleViewModel()
                         {
                             Name = db.Role
                         }).ToList()
            };
            return user;
        }


        public List<RoleViewModel> Getroleslist()
        {










            // var roles = _context.Roles.ToList();
            var asd = (from b in _context.Roles
                       select new RoleViewModel()
                       {
                           Id = b.Id,
                           Name = b.Role

                       }).ToList();
            return asd;
        }












    }
}
