using System.Collections.Generic;

namespace Databas.Models {
    // ViewModel for managing budget and its associated expenses
    public class BudgetManageViewModel {
        public BudgetModel Budget { get; set; }
        public List<ExpenseModel> Expenses { get; set; }
        public decimal TotalAmount { get; set; }
        public BudgetManageViewModel() {
            Budget = new BudgetModel();
            Expenses = new List<ExpenseModel>();
        }
    }
}