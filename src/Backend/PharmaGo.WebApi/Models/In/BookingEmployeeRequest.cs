namespace PharmaGo.WebApi.Models.In;

public class BookingEmployeeRequest
{
    public int PharmacyId { get; set; }
    public string ApproverName { get; set; }
    public int BookingId { get; set; }
    public bool Cancel { get; set; }
}