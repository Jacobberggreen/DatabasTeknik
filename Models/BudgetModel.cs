public class BudgetModel
{
	public int Id { get; set; }
	public string Name { get; set; }	
	public decimal Limit { get; set; }

	// G: Constructor with default values
    // This makes sure the model has reasonable defaults when created without parameters.
    public BudgetModel()
    {
        Id = 0;
        Name = "Unnamed budget";
        Limit = 0m;
    }

    // G: Constructor that takes all attributes as parameters
    // This matches the columns in your table: Id, Name, Limit.
    public BudgetModel(int id, string name, decimal limit)
    {
        Id = id;
        Name = name;
        Limit = limit;
    }
}