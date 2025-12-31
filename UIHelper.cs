using System;

namespace Odev;

public static class UIHelper
{
    public static void PrintHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================");
        Console.WriteLine($"   {title.ToUpper()}");
        Console.WriteLine("========================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintMenu(string[] options)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✔ {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n✖ {message}");
        Console.ResetColor();
    }

    public static string GetInput(string prompt)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine();
        return input?.Trim() ?? string.Empty;
    }

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nDevam etmek için bir tuşa basın...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
