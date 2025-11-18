using Microsoft.AspNetCore.Mvc;
using Databas.Models;
using System.Collections.Generic;
using System.Linq;

namespace Databas.Controllers
{
    public class BudgetController : Controller
    {
        // Default budget settings used if nothing is stored in session yet
        private const string DefaultBudgetName = "Studentbudget";
        private const decimal DefaultBudgetLimit = 13500m;

        // Helper to read budget name and limit from session, with sensible defaults
        private (string Name, decimal Limit) GetBudgetFromSession()
        {
            var budgetName = HttpContext.Session.GetString("BudgetName") ?? DefaultBudgetName;
            var budgetLimitString = HttpContext.Session.GetString("BudgetLimit");

            decimal budgetLimit = DefaultBudgetLimit;
            if (!string.IsNullOrEmpty(budgetLimitString) && decimal.TryParse(budgetLimitString, out var parsedLimit))
            {
                budgetLimit = parsedLimit;
            }

            return (budgetName, budgetLimit);
        }

        // Fake "database" stored in memory while the app is running
        private static List<ExpenseModel> expenses = new List<ExpenseModel>
        {
             // Fixed monthly costs
			new ExpenseModel { Id = 1, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 4223, Description = "Hyra" },
			new ExpenseModel { Id = 2, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 700, Description = "Garage" },
			new ExpenseModel { Id = 3, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 519, Description = "Gymkort" },
			new ExpenseModel { Id = 4, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 520, Description = "Busskort" },
			new ExpenseModel { Id = 5, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 184, Description = "Tvättmedel" },
			new ExpenseModel { Id = 6, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 89, Description = "Schampo" },

			// Subscriptions
			new ExpenseModel { Id = 7, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 89, Description = "Disney+" },
			new ExpenseModel { Id = 8, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 119, Description = "Netflix" },
			new ExpenseModel { Id = 9, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 89, Description = "Viaplay" },
			new ExpenseModel { Id = 10, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 499, Description = "TV4 Play Premium" },

			// Car-related
			new ExpenseModel { Id = 11, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 1000, Description = "Bensin" },
			new ExpenseModel { Id = 12, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 76, Description = "Försäkring lägenhet" },
			new ExpenseModel { Id = 13, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 567, Description = "Bilförsäkring" },
			new ExpenseModel { Id = 14, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 287, Description = "Bilskatt" },

			// Savings
			new ExpenseModel { Id = 15, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 800, Description = "Sparande Avanza" },

			// Groceries (approx total < 4000kr)
			new ExpenseModel { Id = 16, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 732, Description = "ICA storhandling" },
			new ExpenseModel { Id = 17, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 612, Description = "COOP mellanstor handling" },
			new ExpenseModel { Id = 18, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 899, Description = "WILLYS storhandling" },
			new ExpenseModel { Id = 19, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 480, Description = "ICA komplettering" },
			new ExpenseModel { Id = 20, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 350, Description = "WILLYS småhandling" },

			// Coffee & energy drinks, 11 st
			new ExpenseModel { Id = 21, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 35, Description = "Kaffe Espresso House" },
			new ExpenseModel { Id = 22, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 48, Description = "Nocco Caribbean" },
			new ExpenseModel { Id = 23, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 59, Description = "Celsius Peach Vibe" },
			new ExpenseModel { Id = 24, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 27, Description = "Liten kaffe McDonald's" },
			new ExpenseModel { Id = 25, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 39, Description = "Red Bull 250ml" },
			new ExpenseModel { Id = 26, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 45, Description = "Monster Mango Loco" },
			new ExpenseModel { Id = 27, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 29, Description = "Kaffe Pressbyrån" },
			new ExpenseModel { Id = 28, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 69, Description = "Starbucks Doubleshot" },
			new ExpenseModel { Id = 29, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 22, Description = "7-Eleven kaffe" },
			new ExpenseModel { Id = 30, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 55, Description = "Nocco Blood Orange" },
			new ExpenseModel { Id = 31, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 33, Description = "Skolcafeteria kaffe" }
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

            // Budget name and limit from session (with shared defaults)
            var budget = GetBudgetFromSession();

            ViewBag.BudgetName = budget.Name;
            ViewBag.BudgetLimit = budget.Limit;
            ViewBag.Remaining = budget.Limit - totalAmount;

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
			var budget = GetBudgetFromSession();

			var vm = new BudgetManageViewModel
			{
				Budget = new BudgetModel
				{
					Id = 1,
					Name = budget.Name,
					Limit = budget.Limit
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
