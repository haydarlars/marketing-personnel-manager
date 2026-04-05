using System.ComponentModel.DataAnnotations;

namespace MarketingApp.API.DTOs
{
    // ---------------------------------------------------------------
    // Personnel DTOs
    // DTOs (Data Transfer Objects) decouple the API contract from the
    // internal EF models – good practice for production applications.
    // ---------------------------------------------------------------

    /// <summary>Read-only view of a personnel record returned by GET endpoints.</summary>
    public class PersonnelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payload used for both POST (create) and PUT (update) personnel requests.
    /// DataAnnotations here provide model-level validation independent of the EF model.
    /// </summary>
    public class PersonnelUpsertDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(19, 120, ErrorMessage = "Age must be greater than 18.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [MinLength(1, ErrorMessage = "Phone cannot be empty.")]
        [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;
    }

    // ---------------------------------------------------------------
    // Sales DTOs
    // ---------------------------------------------------------------

    /// <summary>Read-only view of a single sales record.</summary>
    public class SaleDto
    {
        public int Id { get; set; }
        public int PersonnelId { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal SalesAmount { get; set; }
    }

    /// <summary>
    /// Payload for creating a new sales record (POST only – no PUT/PATCH allowed).
    /// </summary>
    public class SaleCreateDto
    {
        [Required(ErrorMessage = "Report date is required.")]
        public DateTime ReportDate { get; set; }

        [Required(ErrorMessage = "Sales amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Sales amount cannot be negative.")]
        public decimal SalesAmount { get; set; }
    }
}
