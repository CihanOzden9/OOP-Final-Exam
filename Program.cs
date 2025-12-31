using System;

namespace Odev;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
            
            var authService = new AuthService(dbHelper);
            var productService = new ProductService(dbHelper);
            var rentalService = new RentalService(dbHelper);
            var adminService = new AdminService(dbHelper);
            
            while (true)
            {
                try
                {
                    if (!Session.IsLoggedIn())
                    {
                        ShowWelcomeMenu(authService);
                    }
                    else
                    {
                        if (Session.CurrentUser?.Role == "Admin")
                        {
                            ShowAdminMenu(adminService);
                        }
                        else
                        {
                            ShowUserMenu(dbHelper, productService, rentalService);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrintError($"Beklenmeyen bir hata oluştu: {ex.Message}");
                    UIHelper.Pause();
                }
            }
        }
        catch (Exception ex)
        {
             // Critical database init error or similar
             Console.WriteLine($"KRİTİK HATA: {ex.Message}");
        }
    }

    static void ShowWelcomeMenu(AuthService authService)
    {
        UIHelper.PrintHeader("Rent-A-Tech Kiralama Sistemi");
        UIHelper.PrintMenu(new[] { "Giriş Yap", "Kayıt Ol", "Çıkış" });
        
        var choice = UIHelper.GetInput("Seçiminiz");

        switch (choice)
        {
            case "1":
                LoginFlow(authService);
                break;
            case "2":
                RegisterFlow(authService);
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                UIHelper.PrintError("Geçersiz seçim!");
                UIHelper.Pause();
                break;
        }
    }

    static void LoginFlow(AuthService authService)
    {
        UIHelper.PrintHeader("Giriş Yap");
        string username = UIHelper.GetInput("Kullanıcı Adı");
        string password = UIHelper.GetInput("Şifre");

        if (authService.Login(username, password)) {
            UIHelper.PrintSuccess("Giriş yapıldı! Yönlendiriliyorsunuz...");
            System.Threading.Thread.Sleep(1000);
        } else {
            UIHelper.Pause();
        }
    }

    static void RegisterFlow(AuthService authService)
    {
        UIHelper.PrintHeader("Kayıt Ol");
        string username = UIHelper.GetInput("Kullanıcı Adı");
        string password = UIHelper.GetInput("Şifre");

        if (authService.Register(username, password)) {
             UIHelper.PrintSuccess("Kayıt tamamlandı. Şimdi giriş yapabilirsiniz.");
             UIHelper.Pause();
        } else {
             UIHelper.Pause();
        }
    }

    static void ShowAdminMenu(AdminService adminService)
    {
        UIHelper.PrintHeader("Admin Paneli");
        UIHelper.PrintMenu(new[] { 
            "Dashboard (İstatistikler)", 
            "Kullanıcıları Yönet", 
            "Ürünleri Yönet", 
            "Oturumu Kapat" 
        });

        var choice = UIHelper.GetInput("Seçiminiz");

        switch (choice)
        {
            case "1":
                UIHelper.PrintHeader("Dashboard");
                adminService.ShowDashboardStats();
                UIHelper.Pause();
                break;
            case "2":
                UIHelper.PrintHeader("Kullanıcı Yönetimi");
                adminService.ListAllUsers();
                Console.WriteLine();
                string uidInput = UIHelper.GetInput("Silmek istediğiniz Kullanıcı ID (İptal: Enter)");
                if (int.TryParse(uidInput, out int uid))
                {
                    adminService.DeleteUser(uid);
                    UIHelper.Pause();
                }
                break;
            case "3":
                UIHelper.PrintHeader("Ürün Yönetimi");
                adminService.ListAllProducts();
                Console.WriteLine();
                string pidInput = UIHelper.GetInput("Silmek istediğiniz Ürün ID (İptal: Enter)");
                if (int.TryParse(pidInput, out int pid))
                {
                    adminService.DeleteProduct(pid);
                    UIHelper.Pause();
                }
                break;
            case "4":
                Session.Logout();
                break;
            default:
                UIHelper.PrintError("Geçersiz seçim!");
                UIHelper.Pause();
                break;
        }
    }

    static void ShowUserMenu(DatabaseHelper dbHelper, ProductService productService, RentalService rentalService)
    {
        var user = Session.CurrentUser;
        UIHelper.PrintHeader($"Kullanıcı Paneli ({user?.Username})");
        
        UIHelper.PrintMenu(new[] { 
            "Ürün İlanı Ver", 
            "İlanları Görüntüle", 
            "Ürün Kirala", 
            "Kiraladıklarım",
            "Oturumu Kapat" 
        });

        var choice = UIHelper.GetInput("Seçiminiz");

        switch (choice)
        {
            case "1":
                UIHelper.PrintHeader("Yeni İlan Ekle");
                string name = UIHelper.GetInput("Ürün Adı");
                string rateStr = UIHelper.GetInput("Saatlik Ücret");
                
                if (decimal.TryParse(rateStr, out decimal rate))
                {
                    productService.AddProduct(name, rate, user!.Id);
                    UIHelper.PrintSuccess("İlan eklendi.");
                }
                else
                {
                    UIHelper.PrintError("Geçersiz ücret formatı.");
                }
                UIHelper.Pause();
                break;
            case "2":
                UIHelper.PrintHeader("Müsait Ürünler");
                var products = productService.GetAvailableProducts();
                if (products.Count == 0) Console.WriteLine("Listelenecek ürün yok.");
                foreach (var p in products)
                {
                    Console.WriteLine($"[ID: {p.Id}] {p.Name} - {p.HourlyRate} TL/Saat (Sahibi: {p.OwnerName})");
                }
                UIHelper.Pause();
                break;
            case "3":
                UIHelper.PrintHeader("Ürün Kirala");
                string pIdStr = UIHelper.GetInput("Ürün ID");
                if (int.TryParse(pIdStr, out int prodId))
                {
                    string hoursStr = UIHelper.GetInput("Kiralama Süresi (Saat)");
                    if (int.TryParse(hoursStr, out int hours))
                    {
                        rentalService.RentProduct(prodId, user!.Id, hours);
                    }
                    else
                    {
                        UIHelper.PrintError("Geçersiz süre.");
                    }
                }
                else
                {
                     UIHelper.PrintError("Geçersiz ID.");
                }
                UIHelper.Pause();
                break;
            case "4":
                UIHelper.PrintHeader("Kiralama Geçmişim");
                var rentals = rentalService.GetUserRentals(user!.Id);
                if (rentals.Count == 0) Console.WriteLine("Henüz kiralama yapmadınız.");
                foreach(var r in rentals)
                {
                     Console.WriteLine($"• {r.ProductName} ({r.Hours} Saat) -> {r.TotalPrice} TL");
                }
                UIHelper.Pause();
                break;
            case "5":
                Session.Logout();
                break;
            default:
                UIHelper.PrintError("Geçersiz işlem.");
                UIHelper.Pause();
                break;
        }
    }
}
