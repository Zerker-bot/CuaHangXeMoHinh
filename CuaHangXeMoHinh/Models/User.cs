using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CuaHangXeMoHinh.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
