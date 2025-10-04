using System.Collections.Generic;

namespace ShoeCartBackend.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Roles Role { get; set; } = Roles.user;
        public bool IsBlocked { get; set; } = false;

        public Cart? Cart { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public enum Roles
    {
        user,
        admin
    }
}
