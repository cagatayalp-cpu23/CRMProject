using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMProject.Web.TypeCodes
{
    
        public enum ListEnum : byte
        {
            Deadline = 1,
            IsSelected = 2
        }

        public enum PriorityType : byte
        {
            Low = 3,
            Medium = 2,
            High = 1
        }
    
}