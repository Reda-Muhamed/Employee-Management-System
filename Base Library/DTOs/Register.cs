﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base_Library.DTOs
{
    public class Register: AccountBase
    {
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string? FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

    }
}
