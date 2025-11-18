using System.ComponentModel.DataAnnotations;

public class BudgetModel
{
    // Primary key
    public int Id { get; set; }

    // Name of the budget
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    // Max amount for the budget
    [Required]
    [Range(1, 100000000000, ErrorMessage = "Limit must be a number between 1 and 100 Billion.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Enter an amount in whole numbers only, without dots or commas.")]
    public decimal Limit { get; set; }
}
