namespace PharmaGo.WebApi.Models.In;

public class BookingModelRequest
{
    public string BookerEmail { get; set; }
    public string BookingDate { get; set; }
    public string PublicKey { get; set; }
    public bool HasPrescriptionAll { get; set; }
    public List<BookingModelRequestDetail> Details { get; set; }
}