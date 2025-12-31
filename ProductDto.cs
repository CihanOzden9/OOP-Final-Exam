namespace Odev;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string OwnerName { get; set; } = string.Empty;

    public ProductDto(int id, string name, decimal hourlyRate, string ownerName)
    {
        Id = id;
        Name = name;
        HourlyRate = hourlyRate;
        OwnerName = ownerName;
    }
}
