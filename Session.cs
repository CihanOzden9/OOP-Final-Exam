namespace Odev;

public static class Session
{
    public static User? CurrentUser { get; private set; }

    public static void Login(User user)
    {
        CurrentUser = user;
    }

    public static void Logout()
    {
        CurrentUser = null;
    }

    public static bool IsLoggedIn()
    {
        return CurrentUser != null;
    }
}
