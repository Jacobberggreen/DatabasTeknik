using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Databas.DAL;
using Databas.Models;

namespace Databas.Controllers {

	// Controller för hantering av kategorier
    public class CategoryController : Controller {

		// DAL-instans
        private readonly ExpenseDal _dal;

		// Konstruktor
        public CategoryController() {
            _dal = new ExpenseDal();
        }

		// GET: Visa alla kategorier
        [HttpGet]
        public IActionResult Index() {
            var categories = _dal.GetCategories();
            return View("~/Views/Expense/ManageCategories.cshtml", categories);
        }

		// POST: Lägg till ny kategori
        [HttpPost]
        public IActionResult Create(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                ModelState.AddModelError(string.Empty, "Category name is required.");
                var cats = _dal.GetCategories();
                return View("~/Views/Expense/ManageCategories.cshtml", cats);
            }

            _dal.AddCategory(name.Trim());
            return RedirectToAction("Index");
        }

		// GET: Förbered radering av kategori
        [HttpGet]
        public IActionResult Delete(int id) {
            var categories = _dal.GetCategories();
            var category = categories.FirstOrDefault(c => c.Cat_Id == id);

            if (category == null) {
                return RedirectToAction("Index");
            }

            // Inga utgifter kopplade -> radera direkt
            if (!_dal.CategoryHasExpenses(id)) {
                _dal.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            // Det finns utgifter -> visa bekräftelsesidan
            var expenseCount = _dal.GetExpenseCountForCategory(id);
            var otherCategories = categories.Where(c => c.Cat_Id != id).ToList();

            var vm = new CategoryDeleteViewModel {
                CategoryId = id,
                CategoryName = category.Cat_Name,
                ExpenseCount = expenseCount,
                AvailableCategories = otherCategories
            };

            return View("~/Views/Expense/ConfirmDeleteCategory.cshtml", vm);
        }

		// POST: Bekräfta radering / flytt av utgifter
        [HttpPost]
        public IActionResult ConfirmDeleteCategory(int categoryId, string actionType, int? newCategoryId) {
            if (actionType == "move") {
                if (!newCategoryId.HasValue) {
                    // Ingen mål-kategori vald, gå tillbaka till Delete-vyn
                    return RedirectToAction("Delete", new { id = categoryId });
                }

                _dal.MoveExpensesAndDeleteCategory(categoryId, newCategoryId.Value);
            }
            else if (actionType == "delete") {
                _dal.DeleteExpensesAndCategory(categoryId);
            }

            return RedirectToAction("Index");
        }
    }
}