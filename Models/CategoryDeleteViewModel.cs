using System.Collections.Generic;

namespace Databas.Models {

	// ViewModel för borttagning av kategori
    public class CategoryDeleteViewModel {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ExpenseCount { get; set; }
        public List<CategoryModel> AvailableCategories { get; set; } = new();
    }
}