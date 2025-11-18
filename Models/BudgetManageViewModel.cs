using System.Collections.Generic;

namespace Databas.Models
{
    // Combines Budget + Expenses + extra info
    public class BudgetManageViewModel
    {
        public BudgetModel Budget { get; set; }
        public List<ExpenseModel> Expenses { get; set; }
        public decimal TotalAmount { get; set; }

        public BudgetManageViewModel()
        {
            Budget = new BudgetModel();
            Expenses = new List<ExpenseModel>();
        }
    }
}