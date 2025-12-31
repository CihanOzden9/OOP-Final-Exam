using System;

namespace Odev;

public class AuthService
{
    private readonly DatabaseHelper _dbHelper;

    public AuthService(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public bool Register(string username, string password)
    {
        if (_dbHelper.GetUserByUsername(username) != null)
        {
            Console.WriteLine("Hata: Bu kullanıcı adı zaten alınmış.");
            return false;
        }

        string role = "User";
        if (username == "admin" && password == "admin")
        {
            role = "Admin";
        }

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Username, Password, Role) VALUES ($username, $password, $role)";
            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", password);
            command.Parameters.AddWithValue("$role", role);
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Kayıt başarılı! Lütfen giriş yapın.");
        return true;
    }

    public bool Login(string username, string password)
    {
        var user = _dbHelper.GetUserByUsername(username);

        if (user != null && user.Password == password)
        {
            Session.Login(user);
            Console.WriteLine($"Giriş başarılı! Hoşgeldin, {user.Username} ({user.Role}).");
            return true;
        }

        Console.WriteLine("Hata: Kullanıcı adı veya şifre yanlış.");
        return false;
    }
}
