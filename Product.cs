namespace Odev;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public int OwnerId { get; set; }
    public bool IsRented { get; set; }

    public Product() { }

    public Product(int id, string name, decimal hourlyRate, int ownerId)
    {
        Id = id;
        Name = name;
        HourlyRate = hourlyRate;
        OwnerId = ownerId;
        IsRented = false;
    }
}
