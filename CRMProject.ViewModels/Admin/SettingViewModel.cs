using CRMProject.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CRMProject.ViewModels.Admin
{

    public class LoginViewModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class PersonelViewModel
    {
        public string Name { get; set; }
    }
    public class TaskCrudBaseViewModel
    {
        public TaskProblemIdSelectModel ProblemId { get; set; }
        public TaskUserIdSelectModel UserId { get; set; }
        public TaskRoleIdSelectModel RoleId { get; set; }

    }
    public class TaskAddViewModel : TaskCrudBaseViewModel
    {
        public DateTime? Deadline { get; set; }
        public List<StepViewModel> Step { get; set; }
    }
    public class TaskEditViewModel : TaskCrudBaseViewModel
    {
        public int TaskId { get; set; }
        public ICollection<StepViewModel> Step { get; set; }
    }
    public class TaskListViewModel
    {
        public int Id { get; set; }
        public string ProblemName { get; set; }
        public string PersonelName { get; set; }
        public string RoleName { get; set; }
        public bool? IsSelected { get; set; }
        public DateTime? Deadline { get; set; }
        public ICollection<StepViewModel> Step;

        public string AtayaninAdi { get; set; }
        public string GirenKullaniciAdi { get; set; }
        public IEnumerable<ProblemViewModel> ProblemSets;

        public IEnumerable<UserViewModel> Users { get; set; }
        public IEnumerable<RoleViewModel> Roles { get; set; }
        public TaskViewModel taskSet { get; set; }




    }

    public class TaskUserViewModel
    {
        public string role  { get; set; }      
    }
    public class TaskViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ProblemId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
     
        public virtual ProblemViewModel ProblemSet { get; set; }
        public virtual RoleViewModel Roles { get; set; }
        public virtual UserViewModel Users { get; set; }
        public virtual ICollection<StepViewModel> Step { get; set; }
    }

    public class TaskDeleteViewModel
    {
        public int TaskId { get; set; }
    }
    public class TaskProblemIdSelectModel
    {
        [Display(Name = "Problem")]
        public int? ProblemId { get; set; }

    }
    public class TaskUserIdSelectModel
    {
        [Display(Name = "User")]
        public int? UserId { get; set; }

    }
    public class TaskRoleIdSelectModel
    {
        [Display(Name = "Role")]
        public int? RoleId { get; set; }

    }

    public class PURViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ProblemViewModel : PURViewModel
    {

        public TaskProblemIdSelectModel TaskProblemIdSelectModel { get; set; }
    }

    public class NameSelectModel
    {
        public string Name { get; set; }
    }

    public class UserViewModel
    {

        public UserViewModel()
        {
            Name = new List<string>();
        }
        public int? Id { get; set; }
        [Display(Name = "Eklenecek isim")]
        public List<string> Name { get; set; }
        public bool IsSelected { get; set; }
        public TaskUserIdSelectModel TaskUserIdSelectModel { get; set; }
    }
    public class CurrentUserModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public bool IsMainUser { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public long CustomerId { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public IEnumerable<string> AuthCodes { get; set; }

        public bool AuthorizedForAction(string[] authCodes)
        {
            if (IsMainUser || authCodes == null || !authCodes.Any()) return true;
            return authCodes.Any(allowedAuthAction => AuthCodes.Any(a => a == allowedAuthAction)
            );
        }
    }
    public class RoleViewModel 
    {
        public RoleViewModel()
        {
            
        }
        public RoleViewModel(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
          [CanBeNull] public string Name { get; set; }
        public bool IsSelected { get; set; }
        public TaskRoleIdSelectModel TaskRoleIdSelectModel { get; set; }
    }
    public class RoleEditViewModel : PURViewModel
    {
        public int Id { get; set; }
        public List<string> Name { get; set; }
      //  public TaskRoleIdSelectModel TaskRoleIdSelectModel { get; set; }
    }
    public enum ListEnum : byte
    {
        Deadline = 1,
        IsSelected = 5,
        problem=4
    }
    public class TaskSearchViewModel
    {
        public string Name { get; set; }
        public ListEnum SortList { get; set; }

    }

    public class ProblemCountViewModel
    {
        public ProblemCountViewModel(string name)
        {
            Name = name;
            Count = new int();

        }
        public ProblemCountViewModel()
        {

        }
        [CanBeNull] public string Name { get; set; }
        public int Count { get; set; }
    }


}