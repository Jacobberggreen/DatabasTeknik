using System;
using System.ComponentModel.DataAnnotations;

namespace Databas.Models {

	// Modell för Expense-tabellen
    public class ExpenseModel {
        public int Exp_Id { get; set; }

        [Required(ErrorMessage = "Välj en kategori.")]
        public int Exp_FK_Cat { get; set; }

        [Required(ErrorMessage = "Belopp måste anges.")]
        [Range(0.01, 1000000, ErrorMessage = "Beloppet måste vara större än 0.")]
        public decimal Exp_Amount { get; set; }

        [Required(ErrorMessage = "Datum måste anges.")]
        [DataType(DataType.Date)]
        public DateTime Exp_Date { get; set; }

        [MaxLength(255)]
        public string? Exp_Desc { get; set; }

        public string? CategoryName { get; set; }
    }
}