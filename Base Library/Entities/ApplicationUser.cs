﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base_Library.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string ?FullName { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
    }
}
