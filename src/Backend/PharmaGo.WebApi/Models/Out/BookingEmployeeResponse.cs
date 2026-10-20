namespace PharmaGo.WebApi.Models.Out;

public class BookingEmployeeResponse
{
    public bool Success { get; set; }

    public BookingEmployeeResponse(bool success)
    {
        Success = success;
    }
}