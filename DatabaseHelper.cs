using Microsoft.Data.Sqlite;
using System.IO;

namespace Odev;

public class DatabaseHelper
{
    private const string DbName = "rental.db";
    private readonly string _connectionString;

    public DatabaseHelper()
    {
        _connectionString = $"Data Source={DbName}";
    }

    public void InitializeDatabase()
    {
        if (!File.Exists(DbName))
        {
            File.Create(DbName).Close();
        }

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL
                );";

            string createProductsTable = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    HourlyRate DECIMAL NOT NULL,
                    OwnerId INTEGER NOT NULL,
                    IsRented INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY(OwnerId) REFERENCES Users(Id)
                );";

            string createRentalsTable = @"
                CREATE TABLE IF NOT EXISTS Rentals (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    RenterId INTEGER NOT NULL,
                    Hours INTEGER NOT NULL,
                    FOREIGN KEY(ProductId) REFERENCES Products(Id),
                    FOREIGN KEY(RenterId) REFERENCES Users(Id)
                );";

            ExecuteCommand(connection, createUsersTable);
            ExecuteCommand(connection, createProductsTable);
            ExecuteCommand(connection, createRentalsTable);
        }
    }

    private void ExecuteCommand(SqliteConnection connection, string commandText)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }

    public User? GetUserByUsername(string username)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Password, Role FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3)
                    );
                }
            }
        }
        return null;
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
