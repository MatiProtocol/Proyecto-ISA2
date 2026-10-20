namespace PharmaGo.Domain.Entities;

public class BookingDetail
{
    public int Id { get; set; }
    public Booking Booking { get; set; }
    public int BookingId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; }
    public string? PrescriptionImage { get; set; } = null;
}