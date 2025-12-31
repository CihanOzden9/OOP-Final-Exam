namespace Odev;

public class RentalDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Hours { get; set; }
    public decimal TotalPrice { get; set; }

    public RentalDto(int id, string productName, int hours, decimal totalPrice)
    {
        Id = id;
        ProductName = productName;
        Hours = hours;
        TotalPrice = totalPrice;
    }
}
