using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Odev;

public class RentalService
{
    private readonly DatabaseHelper _dbHelper;

    public RentalService(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public void RentProduct(int productId, int renterId, int hours)
    {
        try
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    // 1. Check if product exists and is not rented
                    command.CommandText = "SELECT IsRented FROM Products WHERE Id = $checkId";
                    command.Parameters.AddWithValue("$checkId", productId);
                    var isRentedObj = command.ExecuteScalar();
                    
                    if(isRentedObj == null) {
                         Console.WriteLine("Hata: Ürün bulunamadı.");
                         return;
                    }
                    
                    if (Convert.ToInt32(isRentedObj) == 1) {
                        Console.WriteLine("Hata: Bu ürün zaten kiralanmış.");
                        return;
                    }

                    // 2. Insert Rental
                    command.CommandText = @"
                        INSERT INTO Rentals (ProductId, RenterId, Hours)
                        VALUES ($productId, $renterId, $hours);
                    ";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$productId", productId);
                    command.Parameters.AddWithValue("$renterId", renterId);
                    command.Parameters.AddWithValue("$hours", hours);
                    command.ExecuteNonQuery();

                    // 3. Update Product to IsRented = 1
                    command.CommandText = @"
                        UPDATE Products SET IsRented = 1 WHERE Id = $prodId;
                    ";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("$prodId", productId);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("Kiralama işlemi başarıyla tamamlandı!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }

    public List<RentalDto> GetUserRentals(int userId)
    {
        var rentals = new List<RentalDto>();

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT r.Id, p.Name, r.Hours, (p.HourlyRate * r.Hours) as TotalPrice
                FROM Rentals r
                JOIN Products p ON r.ProductId = p.Id
                WHERE r.RenterId = $userId;
            ";
            command.Parameters.AddWithValue("$userId", userId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rentals.Add(new RentalDto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetDecimal(3)
                    ));
                }
            }
        }

        return rentals;
    }
}
