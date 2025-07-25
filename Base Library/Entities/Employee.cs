﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base_Library.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string ?CivilId { get; set; }
        public string ?FileNumber { get; set; }
        public string ?FullName { get; set; }
        public string ?JobName { get; set; }
        public string ?Address { get; set; }
        public string ?TelephoneNumber { get; set; }
        public string ?Photo { get; set; }
        public string ?Other { get; set; }
        // Relationships : Many to One
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int TownId { get; set; }
        public Town? Town { get; set; }
        public Department? Department { get; set; }
        public int DepartmentId { get; set; }
        public GeneralDepartment? GeneralDepartment { get; set; }
        public int GeneralDepartmentId { get; set; }

    }
}
