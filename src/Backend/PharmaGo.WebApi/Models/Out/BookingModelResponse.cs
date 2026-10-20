namespace PharmaGo.WebApi.Models.Out;

using Domain.Entities;

public class BookingModelResponse
{
    public int Id { get; set; }
    public string BookerEmail { get; set; }
    public string BookingDate { get; set; }
    public string ConfirmationDate { get; set; }
    public string ClosureDate { get; set; }
    public int TotalAmount { get; set; }
    public string PublicKey { get; set; }
    public string Status { get; set; }
    public List<BookingModelResponseDetail> Details { get; set; }

    public BookingModelResponse(Booking booking)
    {
        Id = booking.Id;
        BookerEmail = booking.BookerEmail;
        BookingDate = booking.BookingDate;
        ConfirmationDate = booking.ConfirmationDate;
        ClosureDate = booking.ClosureDate;
        TotalAmount = booking.TotalAmount;
        PublicKey = booking.PublicKey;
        Status = booking.Status.ToString();
        Details = new List<BookingModelResponseDetail>();

        foreach (var drug in booking.Details)
        {
            var drugResponse = new BookingModelResponseDetail()
            {
                Id = drug.Id,
                Code = drug.Code,
                Name = drug.Name,
                Quantity = drug.Quantity,
                Price = drug.Price,
                PharmacyId = drug.PharmacyId,
                PharmacyName = drug.PharmacyName,
                PrescriptionImage = drug.PrescriptionImage
            };
            Details.Add(drugResponse);
        }
    }
}