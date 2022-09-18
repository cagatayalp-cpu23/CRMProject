using CRMProject.Data;
using CRMProject.ViewModels.Admin;
using CRMProject.ViewModels.Common;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using System.Configuration;
using Dapper;
namespace CRMProject.Service.Admin
{
    public class TaskService
    {
        private readonly CrmDbTestEntities _context;

        public TaskService(CrmDbTestEntities context)
        {
            _context = context;
        }

        public async Task<List<TaskListViewModel>> GetCurrentUserTasks()
        {



            var reference = await (from b in _context.TaskUser.Where(x => x.Users.UserName == System.Web.HttpContext.Current.User.Identity.Name || x.Users.UserName == "admin")
                                   select new TaskListViewModel()
                                   {
                                       PersonelName = b.TaskSet.Personels.Name,
                                       ProblemName = b.TaskSet.ProblemSet.Name,
                                       RoleName = b.TaskSet.Roles.Role,
                                       Id = b.taskId,
                                       Deadline = (DateTime?)b.TaskSet.Deadline,
                                       IsSelected = b.TaskSet.IsSelected.Value,
                                       Step = (from x in b.TaskSet.Step

                                               select new StepViewModel()
                                               {
                                                   Id = x.Id,
                                                   StepDetail = x.StepDetail,
                                                   taskId = (int)x.taskId


                                               }).ToList(),
                                       Users = _context.TaskUser.Where(x => x.taskId == b.taskId).GroupBy(x => x.taskId).Select(x => new UserViewModel()
                                       {
                                           Name = x.Select(t => t.Users.UserName).ToList()
                                       })


                                   }).ToListAsync();

            return reference;
        }

        public TaskAddViewModel GetEmptyStep()
        {
            var model = new TaskAddViewModel();
            model.Step = new List<StepViewModel>();
            if (!model.Step.Any())
            {
                model.Step.Add(new StepViewModel());
            }

            return model;
        }
        public List<TaskListViewModel> GetEmptyTasks(string authticket)
        {

            var reference = (from b in _context.TaskSet.Where(x => !x.TaskUser.Any() && x.Roles.Role == authticket)
                             select new TaskListViewModel()
                             {
                                 PersonelName = b.Personels.Name,
                                 ProblemName = b.ProblemSet.Name,
                                 RoleName = b.Roles.Role,
                                 Deadline = (DateTime?)b.Deadline,
                                 Id = b.Id,

                                 Step = (from x in b.Step
                                         select new StepViewModel()
                                         {
                                             Id = x.Id,
                                             StepDetail = x.StepDetail,
                                             taskId = (int)x.taskId,
                                             IsDone = x.IsDone


                                         }).ToList()
                             }).ToList();
            return reference;
        }

        public int EklenenKullanici(UserViewModel model)
        {
            int i = 0;
            string role;
            //    string[] role = new string[100];
            foreach (var item in model.Name)
            {
                var user = _context.TaskUser.FirstOrDefault(x => x.Users.UserName == item);
                role = user.TaskSet.Roles.Role.ToString();
                if (role == "IT" || role == "Manager")
                {
                    i++;
                }

            }

            return i;
        }
        public ServiceCallResult SelectUser(int id, UserViewModel model2, string girenkullanici)
        {
            var model = _context.TaskSet.FirstOrDefault(x => x.Id == id);
            model.AtayanKullanici = girenkullanici;



            foreach (var item in model2.Name)
            {
                //    var user = _context.TaskUser.FirstOrDefault(x => x.Users.UserName == item);

                var users = _context.Users.FirstOrDefault(x => x.UserName == item);
                var userId = users.Id;
                var role = users.Roles.Role;



                model.TaskUser.Add(new TaskUser()
                {
                    Users = users,
                    taskId = id
                });
                model.GirenKullaniciRolu = role;


            }

            var callResult = new ServiceCallResult() { Success = false };


            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    dbTransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = id;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }

            //var reference = await (from b in _context.TaskSet
            //                       select new TaskListViewModel()
            //                       {
            //                           PersonelName = b.Personels.Name,
            //                           ProblemName = b.ProblemSet.Name,
            //                           RoleName = b.Roles.Role,
            //                           UserName = b.Users.UserName,
            //                           Id = b.Id
            //                       }).ToListAsync();
            //return reference;
        }
        //public TaskListViewModel Get4()
        //{
        //    var reference = new TaskListViewModel()
        //    {
        //        //Personels = _context.Personels.ToList(),
        //        ProblemSets = _context.ProblemSet.ToList(),
        //        Roles = _context.Roles.ToList(),
        //        Users = _context.Users.ToList(),
        //        taskSet = new TaskSet()
        //    };
        //    return reference;
        //}
        //public List<TaskListViewModel> AddEmptyTask(TaskSet model, string authticket)
        //{
        //    var role = _context.Users.FirstOrDefault(x => x.Roles.Role == authticket);
        //    model.RoleId = role.RoleId;
        //    model.UserId = null;
        //    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //    _context.TaskSet.Add(model);
        //    _context.SaveChanges();
        //    var reference = (from b in _context.TaskSet.Where(x => x.UserId == null).Where(x => x.RoleId == role.RoleId)
        //                     select new TaskListViewModel()
        //                     {
        //                         PersonelName = b.Personels.Name,
        //                         ProblemName = b.ProblemSet.Name,
        //                         RoleName = b.Roles.Role,
        //                         UserName = b.Users.UserName,
        //                         Deadline = (DateTime?)b.Deadline,
        //                         Step = b.Step,

        //                         Id = b.Id
        //                     }).ToList();
        //    return reference;

        //}
        public ServiceCallResult StepDelete(int id)
        {
            var callresult = new ServiceCallResult() { Success = false };

            var silinecekstep = _context.Step.FirstOrDefault(x => x.Id == id);
            if (silinecekstep != null)
            {
                _context.Step.Remove(silinecekstep);
                callresult.Success = true;
                _context.SaveChanges();
            }

            return callresult;


        }
        public async Task<ServiceCallResult> SilAsync(int taskId, CurrentUserModel currentUser)
        {
            //var model=_context.TaskSet.FirstOrDefault(x=>x.Id==id);   
            var callResult = new ServiceCallResult() { Success = false };
            var task = await _context.TaskSet.FirstOrDefaultAsync(a => a.Id == taskId).ConfigureAwait(false);
            var taskuser = await _context.TaskUser.Where(a => a.taskId == taskId).ToListAsync();
            var taskStep = _context.Step.Where(a => a.taskId == taskId).ToList();
            if (currentUser.UserName != null)
            {

                if (task.AtayanKullanici != currentUser.UserName &&
                    currentUser.UserName != "admin")
                {
                    callResult.ErrorMessages.Add("Yetkiniz yok");
                    return callResult;
                }
            }

            _context.Step.RemoveRange(taskStep);
            _context.TaskUser.RemoveRange(taskuser);

            _context.TaskSet.Remove(task);
            callResult.Success = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return callResult;
        }
        public async Task<TaskEditViewModel> GetProblemViewModelAsync(int taskId)
        {
            // var step = _context.Step.Where(x => x.taskId == taskId).ToList();
            // _context.Step.RemoveRange(step);

            var task = await (from b in _context.TaskSet
                              where b.Id == taskId
                              select new TaskEditViewModel()
                              {
                                  TaskId = b.Id,
                                  ProblemId = new TaskProblemIdSelectModel
                                  {
                                      ProblemId = b.ProblemId
                                  },
                                  RoleId = new TaskRoleIdSelectModel
                                  {
                                      RoleId = b.RoleId
                                  },
                                  UserId = new TaskUserIdSelectModel
                                  {
                                      UserId = b.Id
                                  },
                                  Step = (from c in _context.Step.Where(x => x.taskId == taskId)
                                          select new StepViewModel()
                                          {
                                              taskId = c.taskId,
                                              Id = c.Id,
                                              StepDetail = c.StepDetail,
                                              IsDone = c.IsDone
                                          }).ToList()

                              }).FirstOrDefaultAsync();
            //     _context.SaveChanges();
            return task;
        }
        public async Task<List<ProblemViewModel>> GetProblemViewModelsAsync()
        {
            var task = await (from b in _context.ProblemSet
                              select new ProblemViewModel()
                              {
                                  Id = b.Id,
                                  Name = b.Name
                              }).ToListAsync();
            return task;
        }

        public async Task<List<StepViewModel>> GetStepViewModel(int id)
        {
            var step = await (from b in _context.Step
                              select new StepViewModel()
                              {

                                  StepDetail = b.StepDetail,



                              }).ToListAsync();
            return step;
        }
        //public async Task<TaskEditViewModel> GetRoleViewModelAsync(int roleId)
        //{
        //    var role = await (from b in _context.TaskSet
        //                      where roleId == b.Id
        //                      select new TaskListViewModel()
        //                      {
        //                          Id = b.Roles.Id,
        //                          RoleName = b.Roles.Role,
        //                           = new TaskRoleIdSelectModel()
        //                          {
        //                              RoleId = (int)b.RoleId
        //                          }
        //                      }).FirstOrDefaultAsync();
        //    return role;
        //}
        public async Task<List<RoleViewModel>> GetRoleViewModelsAsync()
        {
            var role = await (from b in _context.Users
                              select new RoleViewModel()
                              {
                                  Id = b.Id,
                                  Name = (b.Roles.Role)


                              }).ToListAsync();
            return role;
        }
        //public async Task<TaskListViewModel> GetUserViewModelAsync(int userId)
        //{
        //    var role = await (from b in _context.TaskSet
        //                      where userId == b.Id
        //                      select new TaskListViewModel()
        //                      {
        //                          Id = b.Id,
        //                          UserName = b.Users.UserName,
        //                          TaskUserIdSelectModel = new TaskUserIdSelectModel()
        //                          {
        //                              UserId = (int)b.UserId
        //                          }
        //                      }).FirstOrDefaultAsync();
        //    return role;
        //}
        public async Task<List<UserViewModel>> GetUserViewModelsAsync()
        {
            var user = await (from b in _context.Users
                              select new UserViewModel()
                              {
                                  Id = b.Id,
                                  Name = new List<string> { b.UserName },


                              }).ToListAsync();
            return user;
        }
        private IQueryable<TaskListViewModel> _getTasksListIQueryable(Expression<Func<Data.TaskSet, bool>> expr)
        {

            var ax = (from b in _context.TaskSet.AsExpandable().Where(expr)
                      select new TaskListViewModel()
                      {
                          PersonelName = b.Personels.Name,
                          ProblemName = b.ProblemSet.Name,
                          RoleName = b.Roles.Role,
                          Id = b.Id,
                          Deadline = (DateTime?)b.Deadline,
                          AtayaninAdi = b.AtayanKullanici,
                          IsSelected = b.IsSelected.Value,
                          GirenKullaniciAdi = b.GirenKullaniciRolu,
                          Step = (from x in b.Step
                                  select new StepViewModel()
                                  {
                                      Id = x.Id,
                                      StepDetail = x.StepDetail,
                                      taskId = x.taskId,
                                      IsDone = x.IsDone

                                  }).ToList(),
                          Users = (from x in b.TaskUser
                                   select new UserViewModel()
                                   {
                                       Id = x.userId,

                                       Name = new List<string> { x.Users.UserName }

                                   }).ToList(),



                      });
            return ax;
        }
        private IQueryable<TaskListViewModel> _getTasksListIQueryable2(Expression<Func<Data.TaskSet, bool>> expr)
        {

            var axa = (from b in _context.TaskSet.AsExpandable().Where(expr)
                       select new TaskListViewModel()
                       {
                           PersonelName = b.Personels.Name,
                           ProblemName = b.ProblemSet.Name,
                           RoleName = b.Roles.Role,
                           Id = b.Id,
                           Deadline = (DateTime?)b.Deadline,
                           IsSelected = b.IsSelected.Value,
                           Step = (from x in b.Step
                                   select new StepViewModel()
                                   {
                                       Id = x.Id,
                                       StepDetail = x.StepDetail,
                                       taskId = (int)x.taskId,
                                       IsDone = x.IsDone

                                   }).ToList(),
                       });
            return axa;
        }
        public async Task<TaskListViewModel> GetTaskListViewAsync2(int taskId)
        {

            var predicate = PredicateBuilder.New<Data.TaskSet>(true);/*AND*/
            predicate.And(a => a.Id == taskId);
            return await _getTasksListIQueryable2(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<TaskListViewModel> GetTaskListViewAsync(int taskId)
        {
            var predicate = PredicateBuilder.New<Data.TaskSet>(true);/*AND*/
            predicate.And(a => a.Id == taskId);
            return await _getTasksListIQueryable(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public IQueryable<TaskListViewModel> GetTasksListIQueryable(TaskSearchViewModel model, CurrentUserModel currentuser)
        {


            var predicate = PredicateBuilder.New<TaskSet>(true);
            // predicate.And(a => a.Id == model..LanguageId);
            predicate.And(x => x.TaskUser.Any(a => a.Users.UserName == currentuser.UserName || a.TaskSet.AtayanKullanici == currentuser.UserName));
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                predicate.And(a => a.ProblemSet.Name.Contains(model.Name));
            }


            return _getTasksListIQueryable(predicate);
        }
        public async Task<ServiceCallResult> AddTaskAsync(TaskAddViewModel model, string authticket)
        {
            var callResult = new ServiceCallResult() { Success = false };

            Roles currentrole = _context.Roles.FirstOrDefault(x => x.Role == authticket);

            var task = new TaskSet()
            {
                ProblemId = model.ProblemId.ProblemId,

                //  RoleId = model.RoleId.RoleId,
                IsSelected = false,
                Roles = currentrole,
                Deadline = model.Deadline

            };

            if (model.Step != null)
            {
                foreach (var item in model.Step)
                {
                    task.Step.Add(new Step()
                    {
                        StepDetail = item.StepDetail,
                        taskId = item.taskId,
                        IsDone = false


                    });
                }
            }

            _context.TaskSet.Add(task);

            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //  await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbTransaction.Commit();
                    //var taskuser = new TaskUser()
                    //{
                    //    taskId = task.Id
                    //};
                    //    _context.TaskUser.Add(taskuser);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    callResult.Success = true;
                    callResult.Item = task.Id;
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }
        }




        public async Task<ServiceCallResult> Edit(TaskEditViewModel model)
        {
            var taskSet = _context.TaskSet.FirstOrDefault(x => x.Id == model.TaskId);
            var step = _context.Step.Where(x => x.taskId == model.TaskId).ToList();
            //   _context.Step.RemoveRange(step);
            //    _context.TaskSet.Add(step);

            taskSet.ProblemId = model.ProblemId.ProblemId;
            if (model.Step != null)
            {


                foreach (var item in model.Step)
                {
                    if (!step.Any(x => x.Id == item.Id))
                    {
                        step.Add(new Step
                        {
                            StepDetail = item.StepDetail,
                            taskId = item.taskId
                        });
                        var eklenecek = step.FirstOrDefault(x => x.StepDetail == item.StepDetail);
                        _context.Step.Add(eklenecek);

                    }


                }
            }

            //    foreach (var item in step)
            //  {
            //    _context.Step.Add(item);
            // }
            //     _context.Step.AddRange(step);
            //         taskSet.UserId = model.UserId.UserId;
            //       taskSet.RoleId = model.RoleId.RoleId;
            var callResult = new ServiceCallResult() { Success = false };

            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    dbtransaction.Commit();
                    callResult.Success = true;
                    callResult.Item = await GetTaskListViewAsync(model.TaskId).ConfigureAwait(false);
                    return callResult;
                }
                catch (Exception exc)
                {
                    callResult.ErrorMessages.Add(exc.GetBaseException().Message);
                    return callResult;
                }
            }

        }

        //public void  KimeEklenecek(List<UserViewModel> model,int id,string role)
        //{
        //    var task = _context.TaskSet.FirstOrDefault(x => x.Id == id);
        //    foreach (var item in model)
        //    {
        //        var user = _context.Users.FirstOrDefault(x => x.UserName == item.Name);
        //        task.Users = user;
        //    }


        //    var callResult = new ServiceCallResult() { Success = false };



        //    //using (var dbtransaction = _context.Database.BeginTransaction())
        //    //{
        //    //    try
        //    //    {
        //    //        await _context.SaveChangesAsync().ConfigureAwait(false);
        //    //        dbtransaction.Commit();
        //    //        callResult.Success = true;
        //    //        callResult.Item = await GetTaskListViewAsync(model.Id).ConfigureAwait(false);
        //    //        return callResult;
        //    //    }
        //    //    catch (Exception exc)
        //    //    {
        //    //        callResult.ErrorMessages.Add(exc.GetBaseException().Message);
        //    //        return callResult;
        //    //    }
        //    //}

        //}

        public ServiceCallResult Change(int id)
        {
            var callresult = new ServiceCallResult() { Success = false };
            var model = _context.TaskSet.Find(id);

            if (model.IsSelected == true)
            {

                if (_context.Step.Where(x => x.taskId == id).All(x => x.IsDone == true))
                {
                    callresult.Success = true;
                    callresult.ErrorMessages.Add("Görevlerin en az biri yapılmamış olmalı!");
                    return callresult;

                }
                model.IsSelected = false;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            else if (model.IsSelected == false)
            {
                foreach (var item in _context.Step.Where(x => x.taskId == id))
                {
                    if (item.IsDone == false)
                    {
                        callresult.Success = true;
                        callresult.ErrorMessages.Add("Görevlerin hepsini tamamlaman gerek!");
                        return callresult;
                    }
                }
                //if (_context.Step.Any(x => x.IsDone == false))
                //{


                //}
                model.IsSelected = true;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();



            }
            return callresult;
            var model2 = _context.TaskSet.Where(x => x.Id == id);



        }

        // private void HttpException(string v)
        //  {
        //      throw new NotImplementedException();
        //  }

        public int SumAsync(string name)
        {
            var model1 = _context.Users.FirstOrDefault(x => x.UserName == name);

            //    var model = _context.TaskSet.Find(model1.taskId);


            int sum;
            foreach (var item in _context.TaskUser.Where(x => x.Users.UserName == name || x.TaskSet.AtayanKullanici == name).Where((x => x.TaskSet.IsSelected == true)))
            {

            }
            foreach (var item in _context.TaskUser.Where(x => x.Users.UserName == name || x.TaskSet.AtayanKullanici == name).Where((x => x.TaskSet.IsSelected == false)))
            {

            }
            var truecount = _context.TaskSet.Count(x => x.TaskUser.Any((t => t.Users.UserName == name && x.IsSelected == true)) || (x.AtayanKullanici == name && x.IsSelected == true));
            var falsecount = _context.TaskSet.Count(x => x.TaskUser.Any((t => t.Users.UserName == name && x.IsSelected == false)) || (x.AtayanKullanici == name && x.IsSelected == false));
            if (truecount + falsecount != 0)
            {
                sum = 100 * (truecount) / (truecount + falsecount);
            }
            else
            {
                sum = 0;
            }
            return sum;
        }

        public string _Atayankullanicialma(int taskId)
        {
            var atayankullanici = _context.TaskSet.FirstOrDefault(x => x.Id == taskId).AtayanKullanici;
            //var atayan = (from b in _context.TaskSet
            //          select new TaskListViewModel()
            //    {
            //        AtayaninAdi = b.AtayanKullanici
            //    }).ToList();
            return atayankullanici;
        }

        public ServiceCallResult IsDone(int id)
        {
            var callresult = new ServiceCallResult() { Success = false };
            var taskchecked = _context.Step.FirstOrDefault(x => x.Id == id);
            if (id == null)
            {
                callresult.ErrorMessages.Add("Böyle bir istek bulunamadı");
                return callresult;
            }

            taskchecked.IsDone = !taskchecked.IsDone;




            using (var dbtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    dbtransaction.Commit();
                    callresult.Success = true;
                    return callresult;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }




        }

        public List<ProblemCountViewModel> piechart(int id)
        {

            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["CrmDbTestConnectionString"].ConnectionString))
            {
                string _sql =
                    " SELECT p.Name, count(*) as Count FROM TaskUser tu INNER JOIN TaskSet t ON t.Id = tu.taskId INNER JOIN ProblemSet p ON p.Id = t.ProblemId WHERE tu.userId = @id GROUP BY p.Name";
                var piechart = (db.Query<ProblemCountViewModel>(_sql, new { id = id })).ToList();
                return piechart;
            }




        }
    }
}


