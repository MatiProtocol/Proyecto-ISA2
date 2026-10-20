namespace PharmaGo.WebApi.Models.In;

public class BookingModelRequestDetail
{
    public int Id { get; set; }
    public string Code { get; set; }
    public int Quantity { get; set; }
    public int PharmacyId { get; set; }
    public string? PrescriptionImage { get; set; } = null;
}