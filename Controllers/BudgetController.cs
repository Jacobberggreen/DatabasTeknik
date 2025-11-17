using Microsoft.AspNetCore.Mvc;
using Databas.Models;
using System.Collections.Generic;

namespace Databas.Controllers
{
    public class BudgetController : Controller
    {
        public IActionResult Index() {
            // Dummy-data som visas istället för databasdata
            var expenses = new List<ExpenseModel>
            {
                new ExpenseModel { Id = 1, UserId = 1, BudgetId = 10, CategoryId = 2, Amount = 150, Description = "Mat på ICA" },
                new ExpenseModel { Id = 2, UserId = 1, BudgetId = 10, CategoryId = 1, Amount = 4500, Description = "Hyra" },
                new ExpenseModel { Id = 3, UserId = 1, BudgetId = 10, CategoryId = 3, Amount = 299, Description = "Nöje - Bio" }
            };

            return View(expenses);
        }

		public IActionResult Add() {
    		return View();
		}

		[HttpPost]
		public IActionResult Add(ExpenseModel expense) {
	
			expense.Id = Random.Shared.Next(1, 9999);
			expense.UserId = 1;
			expense.BudgetId = 10;
			expense.CategoryId = 3;

			return View("AddResult", expense);
		}	


		
    }
}
