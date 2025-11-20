using System.ComponentModel.DataAnnotations;

namespace Databas.Models {

	// Model representing an expense with validation attributes
    public class ExpenseModel {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BudgetId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [Range(1, 1000000, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Description is too long.")]
        public string Description { get; set; } = string.Empty;

    }
}
