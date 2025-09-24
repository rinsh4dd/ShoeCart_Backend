using System;
using System.ComponentModel.DataAnnotations;

namespace ShoeCartBackend.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = String.Empty;

        public string Email { get; set; } = String.Empty;

        public string PasswordHash { get; set; } = String.Empty;

        public Roles Role { get; set; } = Roles.user;
        public bool IsBlocked { get; set; } = false;

        public Cart? Cart { get; set; }
    }

    public enum Roles
    {
        user,
        admin
    }
}
