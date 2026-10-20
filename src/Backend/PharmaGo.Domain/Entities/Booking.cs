using PharmaGo.Domain.Enums;

namespace PharmaGo.Domain.Entities;

public class Booking
{

    public int Id { get; set; }
    public string BookerEmail { get; set; }
    public string BookingDate { get; set; }
    public string ConfirmationDate { get; set; }
    public string ClosureDate { get; set; }
    public int TotalAmount { get; set; }
    public string PublicKey { get; set; }
    public BookingStatus Status { get; set; }
    public List<BookingDetail> Details { get; set; } = null!;
}