using Microsoft.AspNetCore.Mvc;
using Databas.Models;
using System.Collections.Generic;
using System.Linq;

// For session management
namespace Databas.Controllers {

	// BudgetController handles budget and expense management
    public class BudgetController : Controller {
        private const string DefaultBudgetName = "Studentbudget";
        private const decimal DefaultBudgetLimit = 13500m;

        // Reads budget name and limit from session, falling back to default values if nothing is set
        private (string Name, decimal Limit) GetBudgetFromSession() {
            var budgetName = HttpContext.Session.GetString("BudgetName") ?? DefaultBudgetName;
            var budgetLimitString = HttpContext.Session.GetString("BudgetLimit");

            decimal budgetLimit = DefaultBudgetLimit;
            if (!string.IsNullOrEmpty(budgetLimitString) && decimal.TryParse(budgetLimitString, out var parsedLimit)) {
                budgetLimit = parsedLimit;
            }

            return (budgetName, budgetLimit);
        }

		// In-memory “fake database” for demonstration purposes
        private static List<ExpenseModel> expenses = new List<ExpenseModel> {
			// Fixed example data (simulating stored expenses)
			new ExpenseModel { Id = 1, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 4223, Description = "Hyra" },
			new ExpenseModel { Id = 2, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 700, Description = "Garage" },
			new ExpenseModel { Id = 3, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 519, Description = "Gymkort" },
			new ExpenseModel { Id = 4, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 520, Description = "Busskort" },
			new ExpenseModel { Id = 5, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 184, Description = "Tvättmedel" },
			new ExpenseModel { Id = 6, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 89, Description = "Schampo" },

			new ExpenseModel { Id = 7, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 89, Description = "Disney+" },
			new ExpenseModel { Id = 8, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 119, Description = "Netflix" },
			new ExpenseModel { Id = 9, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 89, Description = "Viaplay" },
			new ExpenseModel { Id = 10, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 499, Description = "TV4 Play Premium" },

			new ExpenseModel { Id = 11, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 1000, Description = "Bensin" },
			new ExpenseModel { Id = 12, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 76, Description = "Försäkring lägenhet" },
			new ExpenseModel { Id = 13, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 567, Description = "Bilförsäkring" },
			new ExpenseModel { Id = 14, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 287, Description = "Bilskatt" },

			new ExpenseModel { Id = 15, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 800, Description = "Sparande Avanza" },

			new ExpenseModel { Id = 16, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 732, Description = "ICA storhandling" },
			new ExpenseModel { Id = 17, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 612, Description = "COOP mellanstor handling" },
			new ExpenseModel { Id = 18, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 899, Description = "WILLYS storhandling" },
			new ExpenseModel { Id = 19, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 480, Description = "ICA komplettering" },
			new ExpenseModel { Id = 20, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 350, Description = "WILLYS småhandling" },

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

		// GET: /Budget/
        public IActionResult Index(string sortOrder, int? categoryFilter, bool hideSummary = false) {
            IEnumerable<ExpenseModel> query = expenses;

            // Apply category filter if selected
            if (categoryFilter.HasValue) {
                query = query.Where(e => e.CategoryId == categoryFilter.Value);
            }

            // Apply sorting based on query parameter
            switch (sortOrder) {
                case "cheapest":
                    query = query.OrderBy(e => e.Amount);
                    break;
                case "expensive":
                    query = query.OrderByDescending(e => e.Amount);
                    break;
                case "newest":
                    query = query.OrderByDescending(e => e.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(e => e.Id);
                    break;
                default:
                    break;
            }

            var list = query.ToList();

            // Calculate totals for the current filtered list
            decimal totalAmount = list.Sum(e => e.Amount);
            int count = list.Count;

            ViewBag.TotalAmount = totalAmount;
            ViewData["ExpenseCount"] = count;

            // Retrieve information about last added expense from session
            var lastAmount = HttpContext.Session.GetString("LastAmount");
            var lastDescription = HttpContext.Session.GetString("LastDescription");

            ViewBag.LastAmount = lastAmount;
            ViewBag.LastDescription = lastDescription;

            // Load budget settings from session
            var budget = GetBudgetFromSession();

            ViewBag.BudgetName = budget.Name;
            ViewBag.BudgetLimit = budget.Limit;
            ViewBag.Remaining = budget.Limit - totalAmount;

            ViewBag.HideSummary = hideSummary;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentCategoryFilter = categoryFilter;

            // Send processed list to the view
            return View(list);
        }

		// GET: /Budget/Add
        public IActionResult Add() {
            // Render the empty form for adding a new expense
            return View();
        }

		// POST: /Budget/Add
        [HttpPost]
        public IActionResult Add(ExpenseModel expense) {
            // Prepare and save the new expense in the in-memory list
            int nextId = expenses.Any() ? expenses.Max(e => e.Id) + 1 : 1;
            expense.Id = nextId;
            expense.UserId = 1; // demo user
            expense.BudgetId = 10; // demo budget

            expenses.Add(expense);

            // Remember the last added expense so it can be shown on the Index page
            HttpContext.Session.SetString("LastAmount", expense.Amount.ToString());
            HttpContext.Session.SetString("LastDescription", expense.Description ?? "");

            // Go back to the main budget overview
            return RedirectToAction("Index");
        }

		// GET: /Budget/Manage
		public IActionResult Manage() {
			var budget = GetBudgetFromSession();

			// Build view model combining budget and expense data
			var vm = new BudgetManageViewModel {
				Budget = new BudgetModel {
					Id = 1,
					Name = budget.Name,
					Limit = budget.Limit
				},
				Expenses = expenses,
				TotalAmount = expenses.Sum(e => e.Amount)
			};

			return View(vm);
		}

		// POST: /Budget/UpdateBudget
		[HttpPost]
		public IActionResult UpdateBudget(BudgetManageViewModel vm) {
			var name = vm.Budget.Name ?? "Studentbudget";
			var limitString = vm.Budget.Limit.ToString();

			// Save updated budget settings in session
			HttpContext.Session.SetString("BudgetName", name);
			HttpContext.Session.SetString("BudgetLimit", limitString);

			return RedirectToAction("Manage");
		}


		// POST: /Budget/Delete
        [HttpPost]
        public IActionResult Delete(int id) {
            // Find expense matching provided ID
            var expenseToRemove = expenses.FirstOrDefault(e => e.Id == id);

            // Remove from in-memory list
            if (expenseToRemove != null) {
                expenses.Remove(expenseToRemove);
            }

            return RedirectToAction("Manage");
        }
    }
}
