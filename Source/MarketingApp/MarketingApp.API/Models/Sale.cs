using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingApp.API.Models
{
    /// <summary>
    /// Represents one day's sales record for a marketing personnel member.
    /// Maps to the Sales table in the database.
    /// Note: Sales records can only be ADDED or DELETED – not edited.
    /// </summary>
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>Foreign key to the Personnel table.</summary>
        [Required]
        public int PersonnelId { get; set; }

        /// <summary>The date for which this sales figure applies.</summary>
        [Required(ErrorMessage = "Report date is required.")]
        public DateTime ReportDate { get; set; }

        /// <summary>Total sales amount for the day (up to 10 digits, 2 decimal places).</summary>
        [Required(ErrorMessage = "Sales amount is required.")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Sales amount cannot be negative.")]
        public decimal SalesAmount { get; set; }

        // Navigation property back to Personnel
        [ForeignKey("PersonnelId")]
        public Personnel? Personnel { get; set; }
    }
}
