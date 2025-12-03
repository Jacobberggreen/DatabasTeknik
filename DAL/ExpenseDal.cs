using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Databas.Models;

namespace Databas.DAL {
	// Data Access Layer för utgifter och kategorier
    public class ExpenseDal {

		// Hämtar anslutningssträngen från appsettings.json
        private string GetConnectionString() {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return builder.GetConnectionString("DefaultConnection");
        }

		// Skapar och returnerar en SqlConnection
        private SqlConnection CreateConnection() {
            return new SqlConnection(GetConnectionString());
        }

		// Hämtar alla kategorier från databasen
        public List<CategoryModel> GetCategories() {
            var list = new List<CategoryModel>();

            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = "SELECT Cat_Id, Cat_Name FROM Tbl_Category ORDER BY Cat_Name";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader()) {

					// Läser varje rad och skapar CategoryModel-objekt
                    while (reader.Read()) {
                        list.Add(new CategoryModel {
                            Cat_Id = reader.GetInt32(0),
                            Cat_Name = reader.GetString(1)
                        });
                    }
                }
            }

            return list;
        }

		// Hämtar utgifter med valfria filter och sortering
        public List<ExpenseModel> GetExpenses(int? categoryId = null, string? searchText = null, string? sortOrder = null) {
            var list = new List<ExpenseModel>();

            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = @"
                    SELECT e.Exp_Id, e.Exp_FK_Cat, e.Exp_Amount, e.Exp_Date, e.Exp_Desc,
                           c.Cat_Name
                    FROM Tbl_Expense e
                    INNER JOIN Tbl_Category c ON e.Exp_FK_Cat = c.Cat_Id
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                // Filter på kategori
                if (categoryId.HasValue) {
                    sql += " AND e.Exp_FK_Cat = @CatId";
                    cmd.Parameters.Add("@CatId", SqlDbType.Int).Value = categoryId.Value;
                }

                // Filter på söktext i beskrivning
                if (!string.IsNullOrWhiteSpace(searchText)) {
                    sql += " AND e.Exp_Desc LIKE @Search";
                    cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 255).Value = $"%{searchText}%";
                }

                // Sortering
                sql += sortOrder switch {
                    "amount_desc" => " ORDER BY e.Exp_Amount DESC",
                    "amount_asc"  => " ORDER BY e.Exp_Amount ASC",
                    "date_asc"    => " ORDER BY e.Exp_Date ASC",
                    _             => " ORDER BY e.Exp_Date DESC"
                };

                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader()) {

					// Läser varje rad och skapar ExpenseModel-objekt
                    while (reader.Read()) {
                        list.Add(new ExpenseModel {
                            Exp_Id = reader.GetInt32(0),
                            Exp_FK_Cat = reader.GetInt32(1),
                            Exp_Amount = reader.GetDecimal(2),
                            Exp_Date = reader.GetDateTime(3),
                            Exp_Desc = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CategoryName = reader.GetString(5)
                        });
                    }
                }
            }

            return list;
        }

		// Lägger till en ny utgift i databasen
        public bool AddExpense(ExpenseModel e) {
            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = @"
                    INSERT INTO Tbl_Expense (Exp_FK_Cat, Exp_Amount, Exp_Date, Exp_Desc)
                    VALUES (@CatId, @Amount, @Date, @Desc);";

                using (var cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.Add("@CatId", SqlDbType.Int).Value = e.Exp_FK_Cat;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = e.Exp_Amount;
                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = e.Exp_Date;
                    cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 255).Value =
                        (object?)e.Exp_Desc ?? DBNull.Value;

                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

		// Hämtar en utgift baserat på dess ID
        public ExpenseModel? GetExpenseById(int id) {
            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = @"
                    SELECT e.Exp_Id, e.Exp_FK_Cat, e.Exp_Amount, e.Exp_Date, e.Exp_Desc,
                           c.Cat_Name
                    FROM Tbl_Expense e
                    INNER JOIN Tbl_Category c ON e.Exp_FK_Cat = c.Cat_Id
                    WHERE e.Exp_Id = @Id";

                using (var cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    using (var reader = cmd.ExecuteReader()) {

						// Om en rad hittas, skapa och returnera ExpenseModel
                        if (reader.Read()) {
                            return new ExpenseModel {
                                Exp_Id = reader.GetInt32(0),
                                Exp_FK_Cat = reader.GetInt32(1),
                                Exp_Amount = reader.GetDecimal(2),
                                Exp_Date = reader.GetDateTime(3),
                                Exp_Desc = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CategoryName = reader.GetString(5)
                            };
                        }
                    }
                }
            }

            return null;
        }

		// Uppdaterar en befintlig utgift i databasen
        public bool UpdateExpense(ExpenseModel e) {
            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = @"
                    UPDATE Tbl_Expense
                    SET Exp_FK_Cat = @CatId,
                        Exp_Amount = @Amount,
                        Exp_Date   = @Date,
                        Exp_Desc   = @Desc
                    WHERE Exp_Id = @Id";

                using (var cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.Add("@CatId", SqlDbType.Int).Value = e.Exp_FK_Cat;
                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = e.Exp_Amount;
                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = e.Exp_Date;
                    cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 255).Value =
                        (object?)e.Exp_Desc ?? DBNull.Value;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = e.Exp_Id;

                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

		// Tar bort en utgift baserat på dess ID
        public bool DeleteExpense(int id) {
            using (var conn = CreateConnection()) {
                conn.Open();

                string sql = "DELETE FROM Tbl_Expense WHERE Exp_Id = @Id;";

                using (var cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

		// Lägger till en ny kategori i databasen
        public bool AddCategory(string name) {
            using (var conn = CreateConnection()) {
                conn.Open();

                using (var cmd = new SqlCommand("INSERT INTO Tbl_Category (Cat_Name) VALUES (@Name)", conn)) {
                    cmd.Parameters.AddWithValue("@Name", name);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

		// Tar bort en kategori baserat på dess ID
        public bool DeleteCategory(int categoryId) {
            using (var conn = CreateConnection()) {
                conn.Open();

                using (var cmd = new SqlCommand("DELETE FROM Tbl_Category WHERE Cat_Id = @Id", conn)) {
                    cmd.Parameters.AddWithValue("@Id", categoryId);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

		// Hämtar totala utgifter per kategori
		public List<CategoryTotalModel> GetCategoryTotals() {
			var result = new List<CategoryTotalModel>();

			using (var conn = CreateConnection()) {
				conn.Open();

				var sql = @"
					SELECT c.Cat_Name, SUM(e.Exp_Amount) AS TotalAmount
					FROM Tbl_Expense e
					INNER JOIN Tbl_Category c ON e.Exp_FK_Cat = c.Cat_Id
					GROUP BY c.Cat_Name
					ORDER BY c.Cat_Name";

				using (var cmd = new SqlCommand(sql, conn))
				using (var reader = cmd.ExecuteReader()) {
					// Läser varje rad och skapar CategoryTotalModel-objekt
					while (reader.Read()) {
						result.Add(new CategoryTotalModel {
							CategoryName = reader.GetString(0),
							TotalAmount = reader.GetDecimal(1)
						});
					}
				}
			}

			return result;
		}

		// Kollar om en kategori har några kopplade utgifter
		public bool CategoryHasExpenses(int categoryId) {
			return GetExpenseCountForCategory(categoryId) > 0;
		}

		// Hämtar antalet utgifter kopplade till en specifik kategori
		public int GetExpenseCountForCategory(int categoryId) {
			using (var conn = CreateConnection()) {
				conn.Open();

				const string sql = "SELECT COUNT(*) FROM Tbl_Expense WHERE Exp_FK_Cat = @CatId";

				using (var cmd = new SqlCommand(sql, conn)) {
					cmd.Parameters.Add("@CatId", SqlDbType.Int).Value = categoryId;

					var result = cmd.ExecuteScalar();
					if (result == null || result == DBNull.Value) {
						return 0;
					}

					return Convert.ToInt32(result);
				}
			}
		}

		// Flyttar utgifter från en kategori till en annan och tar bort den gamla kategorin
		public void MoveExpensesAndDeleteCategory(int oldCategoryId, int newCategoryId) {
			using (var conn = CreateConnection()) {
				conn.Open();

				using (var cmd = new SqlCommand("MoveExpensesAndDeleteCategory", conn)) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@OldCategoryId", SqlDbType.Int).Value = oldCategoryId;
					cmd.Parameters.Add("@NewCategoryId", SqlDbType.Int).Value = newCategoryId;

					cmd.ExecuteNonQuery();
				}
			}
		}

		// Tar bort alla utgifter kopplade till en kategori och sedan kategorin själv
		public void DeleteExpensesAndCategory(int categoryId) {
			using (var conn = CreateConnection()) {
				conn.Open();

				using (var cmd = new SqlCommand("DeleteExpensesAndCategory", conn)) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;

					cmd.ExecuteNonQuery();
				}
			}
		}
    }
}