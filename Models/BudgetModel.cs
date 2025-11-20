using System.ComponentModel.DataAnnotations;

// Model representing a budget with validation attributes
public class BudgetModel {
    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000000000, ErrorMessage = "Limit must be a number between 1 and 100 Billion.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Enter an amount in whole numbers only, without dots or commas.")]
    public decimal Limit { get; set; }
}
