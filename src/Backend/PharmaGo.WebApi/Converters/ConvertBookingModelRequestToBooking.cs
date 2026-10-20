using PharmaGo.Domain.Entities;
using PharmaGo.WebApi.Models.In;

namespace PharmaGo.WebApi.Converters;

public class ConvertBookingModelRequestToBooking
{
    public Booking Convert(BookingModelRequest model)
    {
        var booking = new Booking();
        booking.BookerEmail = model.BookerEmail;
        booking.BookingDate = model.BookingDate;
        booking.PublicKey = model.PublicKey;
        booking.Details = new List<BookingDetail>();

        foreach (var drug in model.Details)
        {
            booking.Details
                .Add(new BookingDetail
                {
                    Id = drug.Id,
                    Code = drug.Code,
                    Name = "",
                    PharmacyId = drug.PharmacyId,
                    PharmacyName = "",
                    PrescriptionImage = drug.PrescriptionImage,
                    Price = 0,
                    Quantity = drug.Quantity,
                });
        }

        return booking;
    }
}