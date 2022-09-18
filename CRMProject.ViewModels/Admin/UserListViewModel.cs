using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMProject.Data;

namespace CRMProject.ViewModels.Admin
{
    public class UserListViewModel
    {
        public int Id { get; set; }
        [Display(Name = "İsim")]
        public string Name { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public IEnumerable<RoleEditViewModel> Roles { get; set; }
        public IEnumerable<UserAddViewModel> Roles2 { get; set; }
    }

    public class UserAddViewModel
    {
        public UserAddViewModel()
        {
            Role = new List<string>();
            Roles = new List<RoleViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Role { get; set; }
        public string Password { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
    public class UserDeleteViewModel
    {
        public int Id { get; set; }
       
    }
    public enum UserListEnum : byte
    {
        UserName = 1,
        Role = 5
    }
    public class UserSearchViewModel
    {
        public string Name { get; set; }
        public UserListEnum SortList { get; set; }
    }

    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            Role = new List<string>();
            IsSelected = new bool();
        }
        public int? Id { get; set; }  
        public string Name { get; set; }
        public List<string> Role { get; set; }
        public string Password { get; set; }
        public List<RoleViewModel> Roles { get; set; }
        public int roleId { get; set; }
        public string OldPassword { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
   
}
