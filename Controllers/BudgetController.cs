using Microsoft.AspNetCore.Mvc;
using Databas.Models;
using System.Collections.Generic;
using System.Linq;

namespace Databas.Controllers
{
    public class BudgetController : Controller
    {
        // Fake "database" stored in memory while the app is running
        private static List<ExpenseModel> expenses = new List<ExpenseModel>
        {
            new ExpenseModel { Id = 1, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 150, Description = "Mat på ICA" },
            new ExpenseModel { Id = 2, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 4500, Description = "Hyra" },
            new ExpenseModel { Id = 3, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 299, Description = "Nöje - Bio" }
        };

        public IActionResult Index(string sortOrder, int? categoryFilter, bool hideSummary = false)
        {
            // Start from the full expenses list
            IEnumerable<ExpenseModel> query = expenses;

            // Optional filter by category
            if (categoryFilter.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryFilter.Value);
            }

            // Sorting based on dropdown selection
            switch (sortOrder)
            {
                case "cheapest":
                    query = query.OrderBy(e => e.Amount);
                    break;
                case "expensive":
                    query = query.OrderByDescending(e => e.Amount);
                    break;
                case "newest":
                    // Assuming higher Id means newer
                    query = query.OrderByDescending(e => e.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(e => e.Id);
                    break;
                default:
                    // Default: no special sorting
                    break;
            }

            var list = query.ToList();

            decimal totalAmount = list.Sum(e => e.Amount);
            int count = list.Count;

            // ViewBag and ViewData examples
            ViewBag.TotalAmount = totalAmount;
            ViewData["ExpenseCount"] = count;

            // Last expense info from session
            var lastAmount = HttpContext.Session.GetString("LastAmount");
            var lastDescription = HttpContext.Session.GetString("LastDescription");

            ViewBag.LastAmount = lastAmount;
            ViewBag.LastDescription = lastDescription;

            // Budget name and limit from session (with defaults)
            var budgetName = HttpContext.Session.GetString("BudgetName") ?? "Studentbudget";
            var budgetLimitString = HttpContext.Session.GetString("BudgetLimit");
            decimal budgetLimit = 10000m;
            if (!string.IsNullOrEmpty(budgetLimitString) && decimal.TryParse(budgetLimitString, out var parsedLimit))
            {
                budgetLimit = parsedLimit;
            }

            ViewBag.BudgetName = budgetName;
            ViewBag.BudgetLimit = budgetLimit;
            ViewBag.Remaining = budgetLimit - totalAmount;

            // Checkbox: hide or show summary
            ViewBag.HideSummary = hideSummary;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentCategoryFilter = categoryFilter;

            // Pass expenses list as the model
            return View(list);
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Add(ExpenseModel expense)
		{
			int nextId = expenses.Any() ? expenses.Max(e => e.Id) + 1 : 1;
			expense.Id = nextId;
			expense.UserId = 1;
			expense.BudgetId = 10;
			// INTE: expense.CategoryId = 3;  // ta bort denna om den finns kvar

			expenses.Add(expense);

			HttpContext.Session.SetString("LastAmount", expense.Amount.ToString());
			HttpContext.Session.SetString("LastDescription", expense.Description ?? "");

			return RedirectToAction("Index");
		}

		public IActionResult Manage()
		{
			var budgetName = HttpContext.Session.GetString("BudgetName") ?? "Studentbudget";
			var budgetLimitString = HttpContext.Session.GetString("BudgetLimit");
			decimal budgetLimit = 10000m;
			if (!string.IsNullOrEmpty(budgetLimitString) && decimal.TryParse(budgetLimitString, out var parsedLimit))
			{
				budgetLimit = parsedLimit;
			}

			var vm = new BudgetManageViewModel
			{
				Budget = new BudgetModel
				{
					Id = 1,
					Name = budgetName,
					Limit = budgetLimit
				},
				Expenses = expenses,
				TotalAmount = expenses.Sum(e => e.Amount)
			};

			return View(vm);
		}

		[HttpPost]
		public IActionResult UpdateBudget(BudgetManageViewModel vm)
		{
			// Save edited budget values into session so they are reused in Index and Manage
			var name = vm.Budget.Name ?? "Studentbudget";
			var limitString = vm.Budget.Limit.ToString();

			HttpContext.Session.SetString("BudgetName", name);
			HttpContext.Session.SetString("BudgetLimit", limitString);

			// After saving, return to Manage view
			return RedirectToAction("Manage");
		}

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Find the expense with the given id
            var expenseToRemove = expenses.FirstOrDefault(e => e.Id == id);

            if (expenseToRemove != null)
            {
                expenses.Remove(expenseToRemove);
            }

            // After deleting, return to Manage
            return RedirectToAction("Manage");
        }
    }
}
