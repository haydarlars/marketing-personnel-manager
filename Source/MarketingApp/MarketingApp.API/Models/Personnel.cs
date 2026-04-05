using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingApp.API.Models
{
    /// <summary>
    /// Represents a marketing department employee.
    /// Maps directly to the Personnel table in the database.
    /// </summary>
    public class Personnel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>Full name of the personnel member.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Age – must be greater than 18 (enforced in API + DB check constraint).</summary>
        [Required(ErrorMessage = "Age is required.")]
        [Range(19, 120, ErrorMessage = "Age must be greater than 18.")]
        public int Age { get; set; }

        /// <summary>Contact phone number – cannot be empty.</summary>
        [Required(ErrorMessage = "Phone is required.")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        // Navigation property – EF will load related sales records.
        // CascadeDelete is configured in DbContext (OnModelCreating).
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
