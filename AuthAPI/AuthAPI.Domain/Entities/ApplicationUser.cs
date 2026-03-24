using System.ComponentModel.DataAnnotations;
using AuthAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Domain.Entities
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
	    [StringLength(50, ErrorMessage = "Full name must not exceed 50 characters.")]
        public string? FullName { get; set; }

	    public DateTime CreatedAtUtc { get; set; }
	    public DateTime? UpdatedAtUtc { get; set; }
    }
}
