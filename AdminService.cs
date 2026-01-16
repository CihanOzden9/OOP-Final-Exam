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
        // Kendi Kendini Silme Engeli
        if (Session.CurrentUser != null && userId == Session.CurrentUser.Id)
        {
            Console.WriteLine("Hata: Kendi hesabınızı silemezsiniz! İşlem durduruldu.");
            return;
        }

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();

            // SQLite Foreign Key Constraint'i aktif et
            using (var pragmaCmd = connection.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                pragmaCmd.ExecuteNonQuery();
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Bu kullanıcının yaptığı kiralamaları sil (Renter olarak)
                    var deleteRentalsCmd = connection.CreateCommand();
                    deleteRentalsCmd.Transaction = transaction;
                    deleteRentalsCmd.CommandText = "DELETE FROM Rentals WHERE RenterId = $userId";
                    deleteRentalsCmd.Parameters.AddWithValue("$userId", userId);
                    deleteRentalsCmd.ExecuteNonQuery();

                    // 2. Bu kullanıcının sahip olduğu ürünlere ait kiralamaları sil 
                    // (Ürün silinmeden önce o ürüne ait kiralamalar silinmeli)
                    var deleteProductRentalsCmd = connection.CreateCommand();
                    deleteProductRentalsCmd.Transaction = transaction;
                    deleteProductRentalsCmd.CommandText = "DELETE FROM Rentals WHERE ProductId IN (SELECT Id FROM Products WHERE OwnerId = $userId)";
                    deleteProductRentalsCmd.Parameters.AddWithValue("$userId", userId);
                    deleteProductRentalsCmd.ExecuteNonQuery();

                    // 3. Kullanıcının ürünlerini sil (Owner olarak)
                    var deleteProductsCmd = connection.CreateCommand();
                    deleteProductsCmd.Transaction = transaction;
                    deleteProductsCmd.CommandText = "DELETE FROM Products WHERE OwnerId = $userId";
                    deleteProductsCmd.Parameters.AddWithValue("$userId", userId);
                    deleteProductsCmd.ExecuteNonQuery();

                    // 4. Kullanıcıyı sil
                    var deleteUserCmd = connection.CreateCommand();
                    deleteUserCmd.Transaction = transaction;
                    deleteUserCmd.CommandText = "DELETE FROM Users WHERE Id = $userId";
                    deleteUserCmd.Parameters.AddWithValue("$userId", userId);
                    
                    int rows = deleteUserCmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        transaction.Commit();
                        Console.WriteLine("Kullanıcı ve ilişkili tüm veriler (Ürünler, Kiralamalar) başarıyla silindi.");
                    }
                    else
                    {
                        // Kullanıcı bulunamadıysa yapılan yan işlemler geri alınsın
                        transaction.Rollback();
                        Console.WriteLine("Hata: Silinecek kullanıcı bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Hata: Kullanıcı silinemedi. Detay: {ex.Message}");
                }
            }
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
