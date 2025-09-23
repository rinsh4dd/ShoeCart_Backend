using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoeCartBackend.Models
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; } 

        public Roles Role { get; set; } =Roles.user;
        public bool IsBlocked { get; set; } = false;
  
        public Cart Cart { get; set; } 
    }

    public enum  Roles
    {
        user,
        admin
    } 
}

