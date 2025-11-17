public class BudgetModel
{
    // Primary key
    public int Id { get; set; }

    // Name of the budget
    public string Name { get; set; } = string.Empty;

    // Max amount for the budget
    public decimal Limit { get; set; }
}
