public class ExpenseModel
{
	public int Id { get; set; }

	public int UserId { get; set; }
	public int BudgetId { get; set; }
	public int CategoryId { get; set; }
	public decimal Amount { get; set; }
	public string Description { get; set; }
}