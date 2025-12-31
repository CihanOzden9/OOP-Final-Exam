using System;
using System.Collections.Generic;

namespace Odev;

public class AdminService
{
    private readonly DatabaseHelper _dbHelper;

    public AdminService(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public void ShowDashboardStats()
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = "SELECT COUNT(*) FROM Users";
            long userCount = (long)command.ExecuteScalar();

            command.CommandText = "SELECT COUNT(*) FROM Products";
            long productCount = (long)command.ExecuteScalar();

            Console.WriteLine($"\n--- ADMIN DASHBOARD ---");
            Console.WriteLine($"Toplam Kullanıcı: {userCount}");
            Console.WriteLine($"Toplam Ürün: {productCount}");
        }
    }

    public void ListAllUsers()
    {
        Console.WriteLine("\n--- TÜM KULLANICILAR ---");
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Role FROM Users";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader.GetInt32(0)} | Kullanıcı: {reader.GetString(1)} | Rol: {reader.GetString(2)}");
                }
            }
        }
    }

    public void DeleteUser(int userId)
    {
        // Prevent deleting self or critical logic could be added here
        if (userId == Session.CurrentUser?.Id)
        {
            Console.WriteLine("Hata: Kendinizi silemezsiniz.");
            return;
        }

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = $id";
            command.Parameters.AddWithValue("$id", userId);
            
            int rows = command.ExecuteNonQuery();
            if (rows > 0) Console.WriteLine("Kullanıcı silindi.");
            else Console.WriteLine("Kullanıcı bulunamadı.");
        }
    }

    public void ListAllProducts()
    {
        Console.WriteLine("\n--- TÜM ÜRÜNLER (Kiralık & Boşta) ---");
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT p.Id, p.Name, p.HourlyRate, p.IsRented, u.Username 
                FROM Products p 
                JOIN Users u ON p.OwnerId = u.Id";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string status = reader.GetBoolean(3) ? "KİRALANDI" : "MÜSAİT";
                    Console.WriteLine($"ID: {reader.GetInt32(0)} | {reader.GetString(1)} | {reader.GetDecimal(2)} TL | {status} | Sahip: {reader.GetString(4)}");
                }
            }
        }
    }

    public void DeleteProduct(int productId)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE Id = $id";
            command.Parameters.AddWithValue("$id", productId);

            int rows = command.ExecuteNonQuery();
            if (rows > 0) Console.WriteLine("Ürün silindi.");
            else Console.WriteLine("Ürün bulunamadı.");
        }
    }
}
