using Microsoft.AspNetCore.Mvc;
using Databas.DAL;
using Databas.Models;
using System.Linq;
using System.Text.Json;

namespace Databas.Controllers {

	// Controller för hantering av utgifter
    public class ExpenseController : Controller {

		// DAL-instans för databasinteraktion
        private readonly ExpenseDal _dal = new ExpenseDal();

		// GET: Visa lista över utgifter med filter och sortering + diagramdata
        [HttpGet]
		public IActionResult Index(string sortOrder, int? categoryId, string searchText) {
			var categories = _dal.GetCategories();
			var expenses = _dal.GetExpenses(categoryId, searchText, sortOrder);

			// Get data for chart: total per category
			var categoryTotals = _dal.GetCategoryTotals();
			ViewBag.CategoryTotals = categoryTotals;

			ViewBag.Categories = categories;
			ViewBag.CurrentSort = sortOrder;
			ViewBag.CurrentCategory = categoryId;
			ViewBag.CurrentSearch = searchText;

			return View(expenses);
		}

		// GET: Visa formulär för att skapa en ny utgift
        [HttpGet]
        public IActionResult Create() {
            ViewBag.Categories = _dal.GetCategories();
            return View(new ExpenseModel { Exp_Date = DateTime.Today });
        }

		// POST: Hantera skapandet av en ny utgift
        [HttpPost]
        public IActionResult Create(ExpenseModel model) {
            if (!ModelState.IsValid) {
                ViewBag.Categories = _dal.GetCategories();
                return View(model);
            }

            bool ok = _dal.AddExpense(model);

            if (!ok) {
                ModelState.AddModelError(string.Empty, "Kunde inte spara utgiften.");
                ViewBag.Categories = _dal.GetCategories();
                return View(model);
            }

            return RedirectToAction("Index");
        }

		// GET: Visa formulär för att redigera en befintlig utgift
        [HttpGet]
        public IActionResult Edit(int id) {
			ModelState.Clear();

            var expense = _dal.GetExpenseById(id);
            if (expense == null) return NotFound();

            ViewBag.Categories = _dal.GetCategories();
            return View(expense);
        }

		// POST: Hantera uppdateringen av en befintlig utgift
        [HttpPost]
        public IActionResult Edit(ExpenseModel model) {
            if (!ModelState.IsValid)  {
                ViewBag.Categories = _dal.GetCategories();
                return View(model);
            }

            bool ok = _dal.UpdateExpense(model);
            if (!ok) {
                ModelState.AddModelError(string.Empty, "Kunde inte uppdatera utgiften.");
                ViewBag.Categories = _dal.GetCategories();
                return View(model);
            }

            return RedirectToAction("Index");
        }

		// POST: Hantera borttagning av en utgift
        [HttpPost]
        public IActionResult Delete(int id) {
            _dal.DeleteExpense(id);
            return RedirectToAction("Index");
        }
    }
}