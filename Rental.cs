namespace Odev;

public class Rental
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int RenterId { get; set; }
    public int Hours { get; set; }

    public Rental() { }

    public Rental(int id, int productId, int renterId, int hours)
    {
        Id = id;
        ProductId = productId;
        RenterId = renterId;
        Hours = hours;
    }
}
