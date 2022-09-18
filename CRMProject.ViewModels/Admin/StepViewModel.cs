using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMProject.ViewModels.Admin
{
    public class StepViewModel
    {
        public bool? IsDone { get; set; }
        public Nullable<int> Id { get; set; }
        public  Nullable<int> taskId { get; set; }
        [Display(Name = "Görev Detayı")]
        public string StepDetail { get; set; }
        public ICollection<StepViewModel> Step { get; set; }
    }

}
