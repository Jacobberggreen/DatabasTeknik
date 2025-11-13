public class BudgetModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Limit { get; set; }

	public BudgetModel()
	{
		Id = 0;
		Name = "Budget1";
		Limit = 13500m;
	}

	public BudgetModel(int id, string name, decimal limit)
	{
		Id = id;
		Name = name;
		Limit = limit;
	}
}



