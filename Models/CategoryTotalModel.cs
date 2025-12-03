namespace Databas.Models {

	// Modell för att representera totalbelopp per kategori
    public class CategoryTotalModel {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}