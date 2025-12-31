using System;
using System.Collections.Generic;

namespace Odev;

public class ProductService
{
    private readonly DatabaseHelper _dbHelper;

    public ProductService(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public void AddProduct(string name, decimal hourlyRate, int ownerId)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Products (Name, HourlyRate, OwnerId, IsRented)
                VALUES ($name, $hourlyRate, $ownerId, 0);
            ";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$hourlyRate", hourlyRate);
            command.Parameters.AddWithValue("$ownerId", ownerId);
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Ürün ilanı başarıyla oluşturuldu.");
    }

    public List<ProductDto> GetAvailableProducts()
    {
        var products = new List<ProductDto>();

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT p.Id, p.Name, p.HourlyRate, u.Username 
                FROM Products p
                JOIN Users u ON p.OwnerId = u.Id
                WHERE p.IsRented = 0;
            ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new ProductDto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDecimal(2),
                        reader.GetString(3)
                    ));
                }
            }
        }

        return products;
    }
}
